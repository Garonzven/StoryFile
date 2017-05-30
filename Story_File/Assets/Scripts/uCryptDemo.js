import System;
import System.IO;
import System.Security;
import System.Security.Cryptography;
import System.Runtime.InteropServices;
import System.Text;
import uCrypt;

var _secretKey = uCrypt.Cryptography.GenerateKey();

function Start()
{
	
            // Encrypt the file.        
            uCrypt.Cryptography.EncryptFile(Application.dataPath + "/NormalFiles/MyData.txt",
               Application.dataPath + "/EncryptedFiles/Encrypted.txt",
               _secretKey);

            // Decrypt the file.
            uCrypt.Cryptography.DecryptFile(Application.dataPath + "/EncryptedFiles/Encrypted.txt",
               Application.dataPath + "/EncryptedFiles/Decrypted.txt",
               _secretKey);
}