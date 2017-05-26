using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Text;
using uCrypt;

public class uCryptDemo : MonoBehaviour {
	// Must be 64 bits, 8 bytes.
    // Distribute this key to the user who will decrypt this file.
	string _secretKey;
	
	// Use this for initialization
	void Start () 
	{
	    // Get the Key for the file to Encrypt.
        _secretKey = uCrypt.Cryptography.GenerateKey();
		
		 // For additional security Pin the key.
            GCHandle gch = GCHandle.Alloc(_secretKey, GCHandleType.Pinned);

            // Encrypt the file.        
            uCrypt.Cryptography.EncryptFile(Application.dataPath + "/NormalFiles/MyData.txt",
               Application.dataPath + "/EncryptedFiles/Encrypted.txt",
               _secretKey);

            // Decrypt the file.
            uCrypt.Cryptography.DecryptFile(Application.dataPath + "/EncryptedFiles/Encrypted.txt",
               Application.dataPath + "/EncryptedFiles/Decrypted.txt",
               _secretKey);

            // Remove the Key from memory. 
            uCrypt.Cryptography.ZeroMemory(gch.AddrOfPinnedObject(), _secretKey.Length * 2);
            gch.Free();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
