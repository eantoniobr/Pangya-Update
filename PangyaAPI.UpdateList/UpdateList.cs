using PangyaAPI.Utilities.Cryptography;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PangyaAPI.UpdateList
{
    public class UpdateList
    {
        //Chaves
        private uint[] _xtea_US_key = { 66455465, 57629246, 17826484, 78315754 }; // 0x3F607A9, 0x36F5A3E, 0x11002B4, 0x4AB00EA
        private uint[] _xtea_JP_key = { 34234324, 32423423, 45336224, 83272673 }; // 0x20A5FD4, 0x1EEBDFF, 0x2B3C6A0, 0x4F6A3E1
        private uint[] _xtea_TH_key = { 84595515, 12254985, 72548314, 46875682 }; // 0x50AD33B, 0x0BAFF09, 0x452FFDA, 0x2CB4422
        private uint[] _xtea_EU_key = { 32081624, 92374137, 64139451, 46772272 }; // 0x1E986D8, 0x5818479, 0x3D2B0BB, 0x2C9B030
        private uint[] _xtea_ID_key = { 23334327, 21322395, 41884343, 93424468 }; // 0x1640DB7, 0x1455A9B, 0x27F1AB7, 0x5918B54
        private uint[] _xtea_KR_key = { 75871606, 85233154, 85204374, 42969558 }; // 0x485B576, 0x5148E02, 0x5141D96, 0x28FA9D6

        public byte[] Document;
        private uint[] DecryptionKey;
        private string FilePath;

        public enum Result
        {
            Sucess,
            Falied,
            Error,
            Test_New_Key
        }
        public enum KeyEnum
        {
            US,
            JP,
            TH,
            EU,
            ID,
            KR
        }

        public enum OperacaoEnum
        {
            Decrypt,
            Encrypt
        }

        public UpdateList(string _filepath)
        {
            this.FilePath = _filepath;
            Document = new byte[0];
        }

        public UpdateList()
        {
        }

        /// <summary>
        /// Construtor com 2 parametros
        /// </summary>
        /// <param name="_filepath">local ou arquivo</param>
        /// <param name="skey">chave do arquivo</param>
        public UpdateList(string _filepath, string skey) : this(_filepath)
        {
            SetDecryptionKey(skey);
        }
        public UpdateList(byte[] data,string _filepath, string skey) : this(_filepath, skey)
        {
            Document = data;
        }

        public string getDocument()
        {
            return Encoding.UTF8.GetString(Document);
        }
        /// <summary>
        /// Sets the decryption key for the updatelist decryption
        /// </summary>
        /// <param name="key">Decryption key</param>
        public void SetDecryptionKey(KeyEnum key)
        {
            if (KeyEnum.US == key)
            {
                DecryptionKey = _xtea_US_key;
            }
            if (KeyEnum.TH == key)
            {
                DecryptionKey = _xtea_TH_key;
            }
            if (KeyEnum.JP == key)
            {
                DecryptionKey = _xtea_JP_key;
            }
            if (KeyEnum.ID == key)
            {
                DecryptionKey = _xtea_ID_key;
            }
            if (KeyEnum.KR == key)
            {
                DecryptionKey = _xtea_KR_key;
            }
            if (KeyEnum.EU == key)
            {
                DecryptionKey = _xtea_EU_key;
            }
            if (DecryptionKey == null)
            {
                throw new Exception("Chave inválida");
            }
        }
        /// <summary>
        /// Sets the decryption key for the updatelist decryption
        /// </summary>
        /// <param name="key">Decryption key</param>

        public void SetDecryptionKey(string key)
        {
            if ("US" == key)
            {
                DecryptionKey = _xtea_US_key;
            }
            if ("TH" == key)
            {
                DecryptionKey = _xtea_TH_key;
            }
            if ("JP" == key)
            {
                DecryptionKey = _xtea_JP_key;
            }
            if ("ID" == key)
            {
                DecryptionKey = _xtea_ID_key;
            }
            if ("KR" == key)
            {
                DecryptionKey = _xtea_KR_key;
            }
            if ("EU" == key)
            {
                DecryptionKey = _xtea_EU_key;
            }
            if (DecryptionKey == null)
            {
                throw new Exception("Chave inválida");
            }
        }

        public void SetFileName(string _file)
        { FilePath = _file; }

        // checar se está encriptado ou decriptografado
        public OperacaoEnum CheckCryptDecrypt(string filePath)
        {

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Arquivo não encontrado");

            //Ler arquivo e convert em char[]
            var dataResult = Encoding.UTF8.GetChars(File.ReadAllBytes(filePath));

            if (dataResult[0] == '^' && dataResult[1] == 'J')
            {
                Debug.WriteLine("UpdateList Decrypt...");
            }
            //verifica se está decriptografado
            // dica o arquivo criptografado inicia com ? >
            if (dataResult[0] == '<' && dataResult[1] == '?')
            {
                Debug.WriteLine("Trying to Encrypt ... \n");
                if (dataResult[0x4B] == 'T' && dataResult[0x4C] == 'H')
                {
                    Debug.WriteLine("Encrypt Key found : Thai ... \n");
                    return OperacaoEnum.Encrypt;
                }
                else if (dataResult[0x4B] == 'J' && dataResult[0x4C] == 'P')
                {
                    Debug.WriteLine("Encrypt Key found : Japan ... \n");
                    return OperacaoEnum.Encrypt;
                }
                else if (dataResult[0x4B] == 'G' && dataResult[0x4C] == 'B')
                {
                    Debug.WriteLine("Encrypt Key found : English ... \n");
                    return OperacaoEnum.Encrypt;
                }
                else if (dataResult[0x4B] == 'E' && dataResult[0x4C] == 'U')
                {
                    Debug.WriteLine("Encrypt Key found : Europe ... \n");
                    return OperacaoEnum.Encrypt;
                }
                else if (dataResult[0x4B] == 'I' && dataResult[0x4C] == 'D')
                {
                    Debug.WriteLine("Encrypt Key found : Indonesia ... \n");
                    return OperacaoEnum.Encrypt;
                }
                else if (dataResult[0x4B] == 'K' && dataResult[0x4C] == 'R')
                {
                    Debug.WriteLine("Encrypt Key found : Korean ... \n");
                    return OperacaoEnum.Encrypt;
                }
                else
                {
                    //no have key :'(
                    Debug.WriteLine("No Key found - Maybe Bad Decrypt :/ ... \n");
                }
            }

            return OperacaoEnum.Decrypt;
        }


        /// <summary>
        /// Decriptografa ou Criptografa Updatelist
        /// </summary>
        /// <param name="operacao">encript ou decript o arquivo</param>
        /// <param name="skey">Chave do updatelist</param>
        /// <param name="decrypted">Decriptografado ou Criptografado</param>
        public bool DecryptFile(string skey = "NONE")
        {
            try
            {
                if (skey != "NONE" || DecryptionKey == null)
                {
                    SetDecryptionKey(skey);
                }
                var _result = new byte[Document.Length];
                if (Document.Count() == 0)
                {

                    using (var inStream = File.OpenRead(FilePath))
                    {
                        try
                        {
                            XTEA.DecipherStreamTrimNull(DecryptionKey, inStream, out Document);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error: {ex.Message}");
                            return false;
                        }
                    }
                }
                else
                {
                    try
                    {
                        XTEA.DecipherStreamTrimNull(DecryptionKey, Document, out _result);
                        Document = _result;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error: {ex.Message}");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Decriptografa ou Criptografa Updatelist
        /// </summary>
        /// <param name="operacao">encript ou decript o arquivo</param>
        /// <param name="skey">Chave do updatelist</param>
        /// <param name="decrypted">Decriptografado ou Criptografado</param>
        public bool EncryptFile(string skey = "NONE")
        {

            try
            {
                if (skey != "NONE" || DecryptionKey == null)
                {
                    SetDecryptionKey(skey);
                }
                var _result = new byte[Document.Length];
                if (Document.Count() == 0)
                {
                    using (var inStream = File.OpenRead(FilePath))
                    {
                        try
                        {
                            XTEA.EncipherStreamPadNull(DecryptionKey, inStream, out _result);
                            Document = _result;
                            File.WriteAllBytes(FilePath, Document);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error: {ex.Message}");
                            return false;
                        }
                    }
                }
                else
                {
                    try
                    {

                        File.WriteAllBytes("update_bck.xml", Document);
                        XTEA.EncipherStreamPadNull(DecryptionKey, Document, out _result);
                        Document = _result;
                        File.WriteAllBytes(FilePath, Document);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error: {ex.Message}");
                        return false;
                    }
                }


                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
    }

}
