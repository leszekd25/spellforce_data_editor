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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFUnPak
{
    public class SFPakHeader
    {
        public int version;
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
            return (version == 4)&&(sentinel[0] == 'M');
        }

        //from pak file
        public void get(BinaryReader br)
        {
            version = br.ReadInt32();
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
            return "VERSION " + version.ToString() + " SENTINEL " + s + " LENGTH "+sentinel.Length.ToString();
        }
    }

    public class SFPakEntryHeader
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
            this.prepare();
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

    public class SFPakFileSystem: IDisposable
    {
        string pak_fname;
        FileStream pak_file;
        BinaryReader pak_stream;
        SFPakHeader pak_header= new SFPakHeader();
        List<SFPakEntryHeader> file_headers = new List<SFPakEntryHeader>();
        uint name_offset = 0;
        uint data_offset = 0;

        public SFPakFileSystem()
        {

        }

        public void Dispose()
        {
            file_headers.Clear();
            Close();
        }

        // returns oath directory, given path string
        // no last backslash
        string GetPathDirectory(string path)
        {
            int index = path.LastIndexOf('\\');
            if (index == -1)
                return "";
            return path.Substring(0, index);
        }

        // returns path filename, given path string
        string GetPathFilename(string path)
        {
            int index = path.LastIndexOf('\\');
            if (index == -1)
                return "";
            return path.Substring(index + 1);
        }

        // returns path file extension, given path string
        //with dot
        string GetPathExtension(string path)
        {
            int index = path.LastIndexOf('.');
            if (index == -1)
                return "";
            return path.Substring(index);
        }

        // initializes pak structure for use from memory
        public int Init(string path)
        {
            FileStream fs;
            try
            {
                fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            }
            catch(Exception e)
            {
                return -2;
            }
            pak_fname = path;
            pak_file = fs;

            pak_stream = new BinaryReader(fs, Encoding.Default);
            pak_header.get(pak_stream);
            if(!pak_header.is_valid())
            {
                fs.Close();
                return -3;
            }

            for(int i = 0; i < pak_header.n_entries; i++)
            {
                file_headers.Add(new SFPakEntryHeader());
                file_headers[i].get(pak_stream);
            }

            data_offset = (uint)pak_header.data_start_offset;
            name_offset = (uint)(92 + 4 * sizeof(int) * file_headers.Count);   //why -4?

            pak_stream.BaseStream.Position = 0;
            //retrieve entry names!
            for (int i = 0; i < file_headers.Count; i++)
            {
                file_headers[i].name = GetFileName(i, false);
                file_headers[i].dir_name = GetFileName(i, true);
            }

            pak_stream.Close();
            fs.Close();
            return 0;
        }

        // opens pak file for reading
        public int Open()
        {
            try
            {
                pak_file = new FileStream(pak_fname, FileMode.Open, FileAccess.Read);
            }
            catch(Exception e)
            {
                return -2;
            }
            pak_stream = new BinaryReader(pak_file, Encoding.Default);
            return 0;
        }

        // closes pak file
        public void Close()
        {
            pak_stream.Close();
            pak_file.Close();
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

        // returns a stream of bytes which constitute for a file given file index
        // if managed_open, it will not try to open a pak, as it assumes the pak is already open
        MemoryStream GetFileBuffer(int file_index, bool managed_open = false)
        {
            if(!managed_open)
                if(Open() != 0)
                    throw new FileLoadException("Could not open pak file "+pak_fname);
            SFPakEntryHeader head = file_headers[file_index];
            uint start = (uint)(data_offset + head.data_offset);
            int length = head.size;

            pak_stream.BaseStream.Position = start;
            Byte[] mem_read = pak_stream.ReadBytes(length);
            MemoryStream ms = new MemoryStream(mem_read);

            if(!managed_open)
                Close();
            return ms;
        }

        // returns a stream of bytes which constitute for a file given file name
        // if managed_open, it will not try to open a pak, as it assumes the pak is already open
        public MemoryStream GetFileBuffer(string fname, bool managed_open = false)
        {
	        string dir_name = GetPathDirectory(fname);
            string file_name = GetPathFilename(fname);
	        for (int i = 0; i<file_headers.Count; i++)
	        {
		        if (file_headers[i].name == file_name && file_headers[i].dir_name == dir_name)
		        {
			        return GetFileBuffer(i, managed_open);
                }
            }
	        return null;
        }

        // writes all pak filedata to a file for later use
        // main reason to do this: faster load times
        public int WriteToFile(BinaryWriter bw)
        {
            bw.Write(pak_fname);
            pak_header.WriteToFile(bw);
            for (int i = 0; i < file_headers.Count; i++)
                file_headers[i].WriteToFile(bw);
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
            file_headers.Clear();
            for (int i = 0; i < pak_header.n_entries; i++)
            {
                SFPakEntryHeader eh = new SFPakEntryHeader();
                eh.ReadFromFile(br);
                file_headers.Add(eh);
            }
            name_offset = br.ReadUInt32();
            data_offset = br.ReadUInt32();
            return 0;
        }

        // returns all files in the given directory with a fitting extension
        public List<String> ListAllWithExtension(string path, string extname)
        {
            List<String> names = new List<string>();
            foreach(SFPakEntryHeader eh in file_headers)
            {
                if ((eh.dir_name == path)&&(GetPathExtension(eh.name) == extname))
                    names.Add(eh.name);
            }
            return names;
        }
    }
}
