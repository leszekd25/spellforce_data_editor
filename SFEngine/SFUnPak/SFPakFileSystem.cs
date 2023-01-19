/*
 * Elbereth's Main Driver - Spellforce Data Archive (.pak) port
 * More at https://github.com/elbereth/DragonUnPACKer
 * 
 * SFPakHeader contains PAK file metadata, including number of entries, data start offset and file size
 * SFPakEntryHeader contains data about file name, file directory and file data offset
 * SFPakFileSystem contains methods to extract binary data from a file of choosing (if it exists),
 *      and methods for listing data based on query
 * */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SFEngine.SFUnPak
{
    public class SFPakHeader
    {
        public byte[] version;
        public char[] sentinel;
        public char[] unknown;
        public int unknown2;
        public int n_entries;
        public int unknown3;
        public int data_start_offset;
        public int file_size;

        public bool is_valid()
        {
            string s = new string(sentinel);
            return ((version[0] == 4) || (version[0] == 5)) && (sentinel[0] == 'M');
        }

        //from pak file
        public void get(BinaryReader br)
        {
            version = br.ReadBytes(4);
            sentinel = br.ReadChars(24);
            unknown = br.ReadChars(44);
            unknown2 = br.ReadInt32();
            n_entries = br.ReadInt32();
            unknown3 = br.ReadInt32();
            data_start_offset = br.ReadInt32();
            file_size = br.ReadInt32();
        }

        //to pak database
        public void WriteToFile(BinaryWriter bw)
        {
            bw.Write(n_entries);
            bw.Write(data_start_offset);
            bw.Write(file_size);
        }

        //from pak database
        public void ReadFromFile(BinaryReader br)
        {
            n_entries = br.ReadInt32();
            data_start_offset = br.ReadInt32();
            file_size = br.ReadInt32();
        }

        public override string ToString()
        {
            string s = new string(sentinel);
            return "VERSION " + version[0].ToString() + " SENTINEL " + s + " LENGTH " + sentinel.Length.ToString();
        }
    }

    public struct SFPakEntryHeader
    {
        public int size;
        public int data_offset;
        public int name_offset;
        public int directory_name_offset;
        public string name;
        public string dir_name;

        void prepare()
        {
            name_offset &= 0xFFFFFF;
            directory_name_offset &= 0xFFFFFF;
        }

        //from pak file
        public void get(BinaryReader br)
        {
            size = br.ReadInt32();
            data_offset = br.ReadInt32();
            name_offset = br.ReadInt32();
            directory_name_offset = br.ReadInt32();
            prepare();
        }

        //to pak database
        public void WriteToFile(BinaryWriter bw)
        {
            bw.Write(size);
            bw.Write(data_offset);
            bw.Write(name);
            bw.Write(dir_name);
        }

        //from pak database
        public void ReadFromFile(BinaryReader br)
        {
            size = br.ReadInt32();
            data_offset = br.ReadInt32();
            name = br.ReadString();
            dir_name = br.ReadString();
        }
    }

    public struct SFPakFileSpan
    {
        public long offset;
        public long size;

        public SFPakFileSpan(long o, long s)
        {
            offset = o;
            size = s;
        }
    }

    public class SFPakFileSystem : IDisposable
    {
        public string pak_fname;                  // filename of the current pakfile
        FileStream pak_file = null;        // filestream for the current pakfile
        BinaryReader pak_stream = null;    // reader for the current pakfile
        SFPakHeader pak_header = new SFPakHeader();        // header containing info about the current pakfile
        SFPakEntryHeader[] file_headers = null;        // list of all entries in the pakfile 

        // file_spans: (string, folder); folder: (string, file)
        // essentially, two level structure: first level is folders, second level is files in the folder
        // this imposes certain constraints: pakfile can only contain files inside a folder
        // however, folders themselves can be nested, example: "mesh" and "mesh\\mesh2" are on the same level, but "mesh" contains "mesh\\mesh2"
        SFPakFileSpan[][] file_spans = null;
        Dictionary<string, int> dir_name_to_folder = null;
        List<Dictionary<string, int>> file_name_to_file = null;
        
        uint name_offset = 0;
        uint data_offset = 0;

        public SFPakFileSystem()
        {

        }

        public void Dispose()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFUnPak, "SFPakFileSystem.Dispose() called, filename: " + pak_fname);

            file_spans = null;
            dir_name_to_folder = null;
            file_name_to_file = null;
            file_headers = null;
            Close();
        }

        // initializes pak structure for use from memory
        public int Init(string path)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFUnPak, "SFPakFileSystem.Init() called, filename: " + path);
            FileStream fs;
            try
            {
                fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            }
            catch (Exception)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFUnPak, "SFPakFileSystem.Init(): Could not open pak file" + path);
                return -2;
            }
            pak_fname = path;
            pak_file = fs;

            pak_stream = new BinaryReader(fs, Encoding.GetEncoding(1252));
            try
            {
                pak_header.get(pak_stream);
            }
            catch (Exception)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFUnPak, "SFPakFileSystem.Init(): Error loading pak header)");
            }
            if (!pak_header.is_valid())
            {
                fs.Close();
                LogUtils.Log.Error(LogUtils.LogSource.SFUnPak, "SFPakFileSystem.Init(): Pak header is invalid!");
                return -3;
            }
            LogUtils.Log.Info(LogUtils.LogSource.SFUnPak, "SFPakFileSystem.Init(): Pak header contents: "
                + String.Format("File count {0}, pak size {1}, pak data offset {2}",
                    pak_header.n_entries.ToString(),
                    pak_header.file_size.ToString(),
                    pak_header.data_start_offset.ToString()));

            file_headers = new SFPakEntryHeader[pak_header.n_entries];
            int ii = 0;
            try
            {
                for (ii = 0; ii < pak_header.n_entries; ii++)
                {
                    file_headers[ii].get(pak_stream);
                }
            }
            catch (Exception)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFUnPak, "SFPakFileSystem.Init(): Error loading filenames (file index " + ii.ToString() + ")");
                fs.Close();
                file_headers = null;
                return -4;
            }

            data_offset = (uint)pak_header.data_start_offset;
            name_offset = (uint)(92 + 4 * sizeof(int) * file_headers.Length);

            dir_name_to_folder = new Dictionary<string, int>();
            file_name_to_file = new List<Dictionary<string, int>>();

            pak_stream.BaseStream.Position = 0;

            // todo: check if the loops can be sped up based on how folder index and file index behave
            for (int i = 0; i < file_headers.Length; i++)
            {
                file_headers[i].name = GetFileName(i, false);
                file_headers[i].dir_name = GetFileName(i, true);

                if (!dir_name_to_folder.ContainsKey(file_headers[i].dir_name))
                {
                    file_name_to_file.Add(new Dictionary<string, int>());
                    dir_name_to_folder.Add(file_headers[i].dir_name, dir_name_to_folder.Count);
                }

                int folder_index = dir_name_to_folder[file_headers[i].dir_name];
                int file_index = file_name_to_file[folder_index].Count;
                file_name_to_file[folder_index].Add(file_headers[i].name, file_index);
            }

            file_spans = new SFPakFileSpan[dir_name_to_folder.Count][];
            foreach(string folder in dir_name_to_folder.Keys)
            {
                int folder_index = dir_name_to_folder[folder];
                file_spans[folder_index] = new SFPakFileSpan[file_name_to_file[folder_index].Count];
            }

            for(int i = 0; i < file_headers.Length; i++)
            {
                int folder_index = dir_name_to_folder[file_headers[i].dir_name];
                int file_index = file_name_to_file[folder_index][file_headers[i].name];
                file_spans[folder_index][file_index] = new SFPakFileSpan(file_headers[i].data_offset, file_headers[i].size);
            }

            Close();
            return 0;
        }

        // returns filename of a file in pak, given by file index in pak
        string GetFileName(int index, bool is_directory = false)
        {
            SFPakEntryHeader head = file_headers[index];
            uint start = name_offset + (is_directory ? (uint)head.directory_name_offset : (uint)head.name_offset + 2);

            pak_stream.BaseStream.Position = start;
            string fname_str = "";

            do
            {
                fname_str += pak_stream.ReadChar();
            }
            while (pak_stream.PeekChar() != '\0');

            char[] charArray = fname_str.ToCharArray();
            Array.Reverse(charArray);
            fname_str = new string(charArray);
            return fname_str;
        }

        // opens pak file for reading
        public int Open()
        {
            if (pak_file != null)
            {
                return 0;
            }

            try
            {
                pak_file = new FileStream(pak_fname, FileMode.Open, FileAccess.Read);
            }
            catch (Exception)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFUnPak, "SFPakFileSystem.Open(): Could not open pak file" + pak_fname);
                return -2;
            }
            pak_stream = new BinaryReader(pak_file, Encoding.GetEncoding(1252));
            return 0;
        }

        // closes pak file
        public void Close()
        {
            if (pak_file == null)
            {
                return;
            }

            pak_stream.Close();
            pak_file.Close();
            pak_stream = null;
            pak_file = null;
        }

        byte[] GetFileBuffer(SFPakFileSpan fspan)
        {
            if (Open() != 0)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFUnPak, "SFPakFileSystem.GetFileBuffer(): Could not open pak file " + pak_fname);
                throw new FileLoadException("Could not open pak file " + pak_fname);
            }

            pak_stream.BaseStream.Position = data_offset + fspan.offset;
            Byte[] mem_read = pak_stream.ReadBytes((int)fspan.size);

            return mem_read;
        }

        // returns a stream of bytes which constitute for a file given file index
        MemoryStream GetFileBuffer(int file_index)
        {
            if (Open() != 0)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFUnPak, "SFPakFileSystem.GetFileBuffer(): Could not open pak file " + pak_fname);
                throw new FileLoadException("Could not open pak file " + pak_fname);
            }
            SFPakEntryHeader head = file_headers[file_index];
            uint start = (uint)(data_offset + head.data_offset);
            int length = head.size;

            pak_stream.BaseStream.Position = start;
            Byte[] mem_read = pak_stream.ReadBytes(length);
            MemoryStream ms = new MemoryStream(mem_read);

            return ms;
        }

        // returns a stream of bytes which constitute for a file given file name
        public byte[] GetFileBuffer(string fname)
        {
            string dir_name = Path.GetDirectoryName(fname);
            int folder_index;

            if (!dir_name_to_folder.TryGetValue(dir_name, out folder_index))
            {
                return null;
            }

            string file_name = Path.GetFileName(fname);
            int file_index;
            if (!file_name_to_file[folder_index].TryGetValue(file_name, out file_index))
            {
                return null;
            }

            return GetFileBuffer(file_spans[folder_index][file_index]);
        }

        // writes all pak filedata to a file for later use
        // main reason to do this: faster load times
        public int WriteToFile(BinaryWriter bw)
        {
            bw.Write(pak_fname);
            pak_header.WriteToFile(bw);
            for (int i = 0; i < file_headers.Length; i++)
            {
                file_headers[i].WriteToFile(bw);
            }

            bw.Write(name_offset);
            bw.Write(data_offset);
            return 0;
        }

        // reads all pak filedata from a file
        // main reason not to use pak files: faster read times
        public int ReadFromFile(BinaryReader br)
        {
            pak_fname = br.ReadString();
            pak_header.ReadFromFile(br);
            file_headers = new SFPakEntryHeader[pak_header.n_entries];

            dir_name_to_folder = new Dictionary<string, int>();
            file_name_to_file = new List<Dictionary<string, int>>();

            // todo: check if the loops can be sped up based on how folder index and file index behave
            for (int i = 0; i < file_headers.Length; i++)
            {
                file_headers[i].ReadFromFile(br);

                if (!dir_name_to_folder.ContainsKey(file_headers[i].dir_name))
                {
                    file_name_to_file.Add(new Dictionary<string, int>());
                    dir_name_to_folder.Add(file_headers[i].dir_name, dir_name_to_folder.Count);
                }

                int folder_index = dir_name_to_folder[file_headers[i].dir_name];
                int file_index = file_name_to_file[folder_index].Count;
                file_name_to_file[folder_index].Add(file_headers[i].name, file_index);
            }

            file_spans = new SFPakFileSpan[dir_name_to_folder.Count][];
            foreach (string folder in dir_name_to_folder.Keys)
            {
                int folder_index = dir_name_to_folder[folder];
                file_spans[folder_index] = new SFPakFileSpan[file_name_to_file[folder_index].Count];
            }

            for (int i = 0; i < file_headers.Length; i++)
            {
                int folder_index = dir_name_to_folder[file_headers[i].dir_name];
                int file_index = file_name_to_file[folder_index][file_headers[i].name];
                file_spans[folder_index][file_index] = new SFPakFileSpan(file_headers[i].data_offset, file_headers[i].size);
            }

            name_offset = br.ReadUInt32();
            data_offset = br.ReadUInt32();
            return 0;
        }

        // returns all files in the given directory with a fitting extension
        public List<String> ListAllWithExtension(string path, string extname)
        {
            List<String> names = new List<string>();
            if (path == "")
            {
                foreach (SFPakEntryHeader eh in file_headers)
                {
                    names.Add(eh.dir_name + "\\" + eh.name);
                }
            }
            else
            {
                foreach(string folder in dir_name_to_folder.Keys)
                {
                    if(folder.StartsWith(path))
                    {
                        int folder_index = dir_name_to_folder[folder];
                        foreach(string file in file_name_to_file[folder_index].Keys)
                        {
                            if(Path.GetExtension(file) == extname)
                            {
                                if (path != folder)
                                {
                                    names.Add(folder.Substring(path.Length + 1, folder.Length - path.Length - 1) + "\\" + file);
                                }
                                else
                                {
                                    names.Add(file);
                                }
                            }
                        }
                    }
                }
            }
            return names;
        }

        public List<string> ListAllWithFilename(string path, string substr)
        {
            List<String> names = new List<string>();
            foreach (string folder in dir_name_to_folder.Keys)
            {
                if (folder == path)
                {
                    int folder_index = dir_name_to_folder[folder];
                    foreach (string file in file_name_to_file[folder_index].Keys)
                    {
                        if (file.Contains(substr))
                        {
                            names.Add(file);
                        }
                    }
                }
            }
            return names;
        }

        public List<string> ListAllInDirectory(string dir)
        {
            List<String> names = new List<string>();
            foreach (string folder in dir_name_to_folder.Keys)
            {
                if (folder == dir)
                {
                    int folder_index = dir_name_to_folder[folder];
                    foreach (string file in file_name_to_file[folder_index].Keys)
                    {
                        names.Add(file);
                    }
                    break;
                }
            }
            return names;
        }
    }
}
