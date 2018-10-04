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

    public class SFPakFileSystem
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

        //no last backslash
        string GetPathDirectory(string path)
        {
            int index = path.LastIndexOf('\\');
            if (index == -1)
                return "";
            return path.Substring(0, index);
        }

        string GetPathFilename(string path)
        {
            int index = path.LastIndexOf('\\');
            if (index == -1)
                return "";
            return path.Substring(index + 1);
        }

        //with dot
        string GetPathExtension(string path)
        {
            int index = path.LastIndexOf('.');
            if (index == -1)
                return "";
            return path.Substring(index);
        }

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

        public void Close()
        {
            pak_stream.Close();
            pak_file.Close();
        }

        string GetFileName(int index, bool is_directory = false)
        {
            SFPakEntryHeader head = file_headers[index];
            uint start = name_offset + (is_directory ? (uint)head.directory_name_offset : (uint)head.name_offset + 2);

            pak_stream.BaseStream.Position = start;
            string fname_str = "";
            
            //System.Diagnostics.Debug.WriteLine("GetFileName " + start.ToString());
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

        MemoryStream GetFileBuffer(int file_index, bool managed_open = false)
        {
            if(!managed_open)
                Open();
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

        //create lookup database
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

        //load lookup database
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

        //query functions
        public List<String> ListAllWithExtension(string extname)
        {
            List<String> names = new List<string>();
            foreach(SFPakEntryHeader eh in file_headers)
            {
                if (GetPathExtension(eh.name) == extname)
                    names.Add(eh.name);
            }
            return names;
        }
    }
}
