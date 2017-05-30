using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FlashDataConverter
{
    class DataConverter
    {
        private static readonly int BLOCK_SIZE_IN_BYTES = 224;
        private List<DataElementDescriptor> _dataDescriptor;

        public DataConverter()
        {

        }

        private byte[] ReadRawDataFromFile(FileStream file, int count, int offset)
        {
            //file.Seek
            byte[] rawData = new byte[count];
            file.Read(rawData, 0, count);
            return rawData;
        }

        public List<DataElementDescriptor> GetDataFromXml(XDocument document)
        {
            List<DataElementDescriptor> elements = new List<DataElementDescriptor>();
            XmlInOrder(elements, document.Root);
            _dataDescriptor = elements;
            return elements;
        }

        private void XmlInOrder(List<DataElementDescriptor> container, XElement element)
        {
            IEnumerable<XAttribute> elementAttributes = element.Attributes();

            if(elementAttributes != null)
            {
                string description = element.Attribute("desc") != null ? element.Attribute("desc").Value : element.Attribute("name") != null ? element.Attribute("name").Value : null;
                string type = element.Attribute("type") != null ? element.Attribute("type").Value : null;
                string units = element.Attribute("units") != null ? element.Attribute("units").Value.ToUpper() : "HEX";
                string calc = element.Attribute("calc") != null ? element.Attribute("calc").Value : null;
                string name = element.Attribute("name") != null ? element.Attribute("name").Value : null;
                string bitNum = element.Attribute("num") != null ? element.Attribute("num").Value : null;


                DataElementDescriptor tempDescriptor = new DataElementDescriptor(name, type, calc, units);
                tempDescriptor.BitNum = bitNum;
                container.Add(tempDescriptor);
            }           

            for (int i = 0; i != element.Elements().Count(); i++)
            {
                XmlInOrder(container, element.Elements().ElementAt(i));
            }
        }

        public List<DataElementDescriptor> GetBlockDataConverted(FileStream file, int blockNum)
        {
            byte[] blockData = ReadRawDataFromFile(file, BLOCK_SIZE_IN_BYTES, blockNum * BLOCK_SIZE_IN_BYTES);

            int currentOffset = 0;            

            for(int i=0; i!=_dataDescriptor.Count; i++)
            {
                if(_dataDescriptor.ElementAt(i).DataLength > 1)
                {
                    int dataLength = (int)(_dataDescriptor.ElementAt(i).DataLength / 8);
                    byte[] dataArray = new byte[dataLength];                    
                    Buffer.BlockCopy(blockData, currentOffset, dataArray, 0, dataLength);
                    Array.Reverse(dataArray);

                    byte[] copyArray = new byte[8];
                    Array.Copy(dataArray, copyArray, dataArray.Length);
                    
                    Int64 tempVal = BitConverter.ToInt64(copyArray, 0);
                    _dataDescriptor.ElementAt(i).RawData = BitConverter.ToInt64(copyArray, 0);                  

                    currentOffset += dataLength;
                    _dataDescriptor.ElementAt(i).SetData(copyArray);
                }
                else if(_dataDescriptor.ElementAt(i).DataLength == 1)
                {
                    int backOffset = Convert.ToInt32(_dataDescriptor.ElementAt(i).BitNum);
                    DataElementDescriptor parent = _dataDescriptor.ElementAt(i - backOffset);
                    _dataDescriptor.ElementAt(i).SetData((int)(parent.RawData >> backOffset) & 0x1);
                }                
            }


            return _dataDescriptor;
        }
        public string GetConvertedLDEDtoString(List<DataElementDescriptor> input)
        {
            string outString = String.Empty;
            foreach(var i in input)
            {
                if ((i.Type != null && i.Type != "description"))
                {
                    outString += i.GetData() + "; ";
                }                
            }

            return outString;
        }
        public string GetHeaders(List<DataElementDescriptor> input)
        {
            string headerString = String.Empty;
            foreach(var v in input)
            {
                if ((v.Type != null && v.Type != "description"))
                {
                    headerString += v.Name + "; ";
                }                
            }

            return headerString;
        }
        public int GetFileLengthInBlocks(FileStream file)
        {
            return (int)Math.Floor((double)(file.Length / BLOCK_SIZE_IN_BYTES));
        }
    }
}
