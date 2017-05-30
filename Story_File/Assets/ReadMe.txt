**********************************************************************************
uCrypt designed by Runningbird Studios 
Jason Burch 2012
Refrence: http://support.microsoft.com/kb/307010
**********************************************************************************

uCrypt can encrypt and decrypt your asset files on the fly from any location.

In the demo I am showing you how to access local files in the Asset folder but you could use a website, server, asset server, etc.

There are 3 methods in the uCrypt library
uCrypt.Cryptography.EncryptFile
uCrypt.Cryptography.DecryptFile
uCrypt.Cryptography.GenerateKey 

You can use your own key if you want but uCrypt will auto generate one for you.

All you need to do is drag the uCryptDemo script onto any game object and set up your encryption and decryption where ever you want.

In the example I am just doing everything under the Start method();

// Get the Key for the file to Encrypt.
 string _secretKey = uCrypt.Cryptography.GenerateKey();
		
// Encrypt the file.        EncryptFile(filepath to file to be encrypted, filepath to new encrypted file, secretKey)
uCrypt.Cryptography.EncryptFile(string, string, string)

// Decrypt the file.       EncryptFile(filepath to file to be encrypted, filepath to new encrypted file, secretKey)
uCrypt.Cryptography.DecryptFile(string, string, string)



Here is more information on the specifics for the Application.dataPath variable I used in the demo.
http://docs.unity3d.com/Documentation/ScriptReference/Application-dataPath.html


	