using System;
using System.Linq;

namespace AsmResolver.PE.DotNet.ReadyToRun
{
    public class PgoInfo
    {
        public PgoInfoKey Key { get; }
        public int Offset { get; }
        public int PgoFormatVersion { get; }
        public byte[] Image { get; }
        private ReadyToRunReader _r2rReader;

        public PgoInfo(PgoInfoKey key, ReadyToRunReader r2rReader, int pgoFormatVersion, byte[] image, int offset)
        {
            PgoFormatVersion = pgoFormatVersion;
            Key = key;
            Offset = offset;
            Image = image;
            _r2rReader = r2rReader;
        }

        // The empty singleton cannot be used for anything except reference equality comparison
        public static PgoInfo EmptySingleton => new PgoInfo(null, null, 0, null, 0);

        PgoSchemaElem[] _pgoData;
        int _size;

        class PgoDataLoader : IPgoSchemaDataLoader<string, string>
        {
            ReadyToRunReader _r2rReader;
            SignatureFormattingOptions _formatOptions;

            public PgoDataLoader(ReadyToRunReader r2rReader, SignatureFormattingOptions formatOptions)
            {
                _formatOptions = formatOptions;
                _r2rReader = r2rReader;
            }

            string IPgoSchemaDataLoader<string, string>.TypeFromLong(long input)
            {
                int tableIndex = checked((int)(input & 0xF));
                int fixupIndex = checked((int)(input >> 4));
                if (tableIndex == 0xF)
                {
                    return $"Unknown type {fixupIndex}";
                }
                else
                {
                    return _r2rReader.ImportSections[tableIndex].Entries[fixupIndex].Signature.ToString(_formatOptions);
                }
            }

            string IPgoSchemaDataLoader<string, string>.MethodFromLong(long input)
            {
                int tableIndex = checked((int)(input & 0xF));
                int fixupIndex = checked((int)(input >> 4));
                if (tableIndex == 0xF)
                {
                    return $"Unknown method {fixupIndex}";
                }
                else
                {
                    return _r2rReader.ImportSections[tableIndex].Entries[fixupIndex].Signature.ToString(_formatOptions);
                }
            }
        }

        void EnsurePgoData()
        {
            if (_pgoData == null)
            {
                if (Image == null)
                {
                    _pgoData = Array.Empty<PgoSchemaElem>();
                    _size = 0;
                }
                else
                {
                    var compressedIntParser = new PgoProcessor.PgoEncodedCompressedIntParser(Image, Offset);

                    SignatureFormattingOptions formattingOptions = new SignatureFormattingOptions();

                    _pgoData = PgoProcessor.ParsePgoData(new PgoDataLoader(_r2rReader, formattingOptions), compressedIntParser, true).ToArray();
                    _size = compressedIntParser.Offset - Offset;
                }
            }
        }

        public PgoSchemaElem[] PgoData
        {
            get
            {
                EnsurePgoData();
                return _pgoData;
            }
        }

        public int Size
        {
            get
            {
                EnsurePgoData();
                return _size;
            }
        }
    }
}
