using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashDataConverter
{
    class DataElementDescriptor
    {
        private string _name;
        private string _type;
        private string _calcString;
        private string _units;

        private string _convertedData;

        public string Name { get { return _name; } }
        public string Type { get { return _type; } }
        public string Units { get { return _units; } }
        public string CalcString { get { return _calcString; } }
        public string BitNum { get; set; }
        public Int64 RawData { get; set; }


        public int DataLength { get; set; } 

        public DataElementDescriptor(string name, string type, string calcString, string units)
        {
            _name = name; _type = type; _calcString = calcString; _units = units; _convertedData = 0.ToString();
            SetDataLength(type);
        }
        public void SetData(byte[] data)
        {
            if(_calcString != null)
            {
                try
                {
                    string dataString = BitConverter.ToInt64(data, 0).ToString();
                    string expression = _calcString.Replace("x", dataString);
                    _convertedData = StLib.RPNCalculator.Calculate(expression).ToString();
                }
                catch(Exception e)
                {
                    _convertedData = "0";
                }                
            }
            else
            {
                _convertedData = BitConverter.ToUInt64(data, 0).ToString();
            }
        }
        public void SetData(int data)
        {
            _convertedData = data.ToString();
        }
        public string GetData()
        {
            return _convertedData;
        }

        private void SetDataLength(string dataType)
        {
            Dictionary<string, int> dataLengthDesc = new Dictionary<string, int>()
            {
                { "unit8"   , 8  },
                { "int8"    , 8  },
                { "uint16"  , 16 },
                { "int16"   , 16 },
                { "uint32"  , 32 },
                { "int32"   , 32 },
                { "uint64"  , 64 },
                { "int64"   , 64 },
                { "bit"     , 1  }
            };

            int length;
            if (dataType != null)
                if (dataLengthDesc.TryGetValue(dataType, out length))
                    DataLength = length;          
        }
    }
}
