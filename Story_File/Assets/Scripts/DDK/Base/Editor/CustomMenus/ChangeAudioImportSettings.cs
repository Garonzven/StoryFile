using UnityEngine;

using UnityEditor;



// /////////////////////////////////////////////////////////////////////////////////////////////////////////

//

// Batch audio import settings modifier.

//

// Modifies all selected audio clips in the project window and applies the requested modification on the

// audio clips. Idea was to have the same choices for multiple files as you would have if you open the

// import settings of a single audio clip. Put this into Assets/Editor and once compiled by Unity you find

// the new functionality in Custom -> Sound. Enjoy! :-)

//

// April 2010. Based on Martin Schultz's texture import settings batch modifier.

// See http://forum.unity3d.com/threads/77816-Changing-import-settings-for-multiple-audio-files

// Modified by Sander H. and downloaded from CaptivatingSound.com - v1.2 September 9, 2013.
// Modified by Jorick B. and downloaded from CaptivatingSound.com - v1.2k May 29, 2014.
// Modified by Sander H. and updated to - v1.3 June 18, 2014.
// Modified by Sander H. and updated to - v1.4 October 9, 2014.

// /////////////////////////////////////////////////////////////////////////////////////////////////////////

public class ChangeAudioImportSettings : ScriptableObject {
	
	static bool deselectAfterCompletion = false; // set to true if you want the script to deselect the current selection after completion


	// ----------------------------------------------------------------------------

	
	[MenuItem ("Custom/Sound/Force to Mono/False")]	
	static void ToggleForceToMono_Auto() {
		
		SelectedToggleForceToMonoSettings(false);
		
	}
	
	
	
	[MenuItem ("Custom/Sound/Force to Mono/True")]	
	static void ToggleForceToMono_Forced() {
		
		SelectedToggleForceToMonoSettings(true);
		
	}
	

	// ----------------------------------------------------------------------------
	
	
	
	[MenuItem ("Custom/Sound/Set Load in Background/True")]	
	static void SetLoadInBg_True() {
		
		SetLoadInBg(true);
		
	}	

	[MenuItem ("Custom/Sound/Set Load in Background/False")]	
	static void SetLoadInBg_False() {
		
		SetLoadInBg(false);
		
	}
	
	
	// ----------------------------------------------------------------------------

	
	[MenuItem ("Custom/Sound/Set Preload Audio Data/True")]	
	static void SetPreloadAudioData_True() {
		
		SetPreloadAudioData(true);
		
	}	
	
	[MenuItem ("Custom/Sound/Set Preload Audio Data/False")]	
	static void SetPreloadAudioData_False() {
		
		SetPreloadAudioData(false);
		
	}
	
	
	// ----------------------------------------------------------------------------
	
	
	[MenuItem ("Custom/Sound/Load type/Stream from disc")]	
	static void ToggleDecompressOnLoad_Disable() {
		
		SelectedToggleDecompressOnLoadSettings(AudioClipLoadType.Streaming);		
	}
	
	
	
	[MenuItem ("Custom/Sound/Load type/Decompress on Load")]	
	static void ToggleDecompressOnLoad_Enable() {
		
		SelectedToggleDecompressOnLoadSettings(AudioClipLoadType.DecompressOnLoad);		
	} 
	
	
	
	[MenuItem ("Custom/Sound/Load type/CompressedInMemory")]	
	static void ToggleDecompressOnLoad_Enable2() {
		
		SelectedToggleDecompressOnLoadSettings(AudioClipLoadType.CompressedInMemory);		
	}

	// ----------------------------------------------------------------------------
	
	[MenuItem ("Custom/Sound/Set audio compression/Vorbis")]	
	static void ToggleCompression_Vorbis() {
		
		SetCompressionSettings( AudioCompressionFormat.Vorbis );
		
	}
	
	[MenuItem ("Custom/Sound/Set audio compression/VAG")]	
	static void ToggleCompression_VAG() {
		
		SetCompressionSettings( AudioCompressionFormat.VAG );
		
	}
	
	[MenuItem ("Custom/Sound/Set audio compression/PCM")]	
	static void ToggleCompression_PCM() {
		
		SetCompressionSettings( AudioCompressionFormat.PCM );
		
	}
	
	[MenuItem ("Custom/Sound/Set audio compression/MP3")]	
	static void ToggleCompression_MP3() {
		
		SetCompressionSettings( AudioCompressionFormat.MP3 );
		
	}
	
	[MenuItem ("Custom/Sound/Set audio compression/HEVAG")]	
	static void ToggleCompression_HEVAG() {
		
		SetCompressionSettings( AudioCompressionFormat.HEVAG );
		
	}
	
	[MenuItem ("Custom/Sound/Set audio compression/ADPCM")]	
	static void ToggleCompression_ADPCM() {
		
		SetCompressionSettings( AudioCompressionFormat.ADPCM );
		
	}
	
	// ----------------------------------------------------------------------------
		
	
	/*[MenuItem ("Custom/Sound/Quality/32")]	
	static void SetCompressionBitrate_32kbps() {
		
		SelectedSetCompressionBitrate(32000);		
	}*/


	// ----------------------------------------------------------------------------

	
	
	/*[MenuItem ("Custom/Sound/Toggle 3D sound/Disable")]
	
	static void Toggle3DSound_Disable() {
		
		SelectedToggle3DSoundSettings(false);
		
	}
	
	
	
	[MenuItem ("Custom/Sound/Toggle 3D sound/Enable")]
	
	static void Toggle3DSound_Enable() {
		
		SelectedToggle3DSoundSettings(true);
		
	}*/
		
	
	
	// ----------------------------------------------------------------------------
	
	/*[MenuItem ("Custom/Sound/Hardware Decoding/Enabled")]
	
	static void enable_Hardware_yes() {
		
		enableHardwareDecoding(true);
		
	}
	
	[MenuItem ("Custom/Sound/Hardware Decoding/Disabled")]
	
	static void enable_Hardware_no() {
		
		enableHardwareDecoding(false);
		
	}*/
	
	// ----------------------------------------------------------------------------
	
	/*[MenuItem ("Custom/Sound/Gapless Looping/Enabled")]
	
	static void enable_Loopable_yes() {
		
		SelectedGaplessLoopingSettings(true);
		
	}
	
	[MenuItem ("Custom/Sound/Gapless Looping/Disabled")]
	
	static void enable_Loopable_no() {
		
		SelectedGaplessLoopingSettings(false);
		
	}*/
	
	
	
	
	
	/*static void enableHardwareDecoding ( bool enable )
		
	{
		
		Object[] audioclips = GetSelectedAudioclips();
		
		if(deselectAfterCompletion) Selection.objects = new Object[0];
		
		foreach (AudioClip audioclip in audioclips) {
			
			string path = AssetDatabase.GetAssetPath(audioclip);
			
			AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
			
			if(audioImporter.hardware != enable)
			{
				audioImporter.hardware = enable;
				AssetDatabase.ImportAsset(path);
			}
		}
		
	}*/
	


	static void SelectedToggleForceToMonoSettings(bool enabled) {
		
		
		Object[] audioclips = GetSelectedAudioclips();
		
		if(deselectAfterCompletion) Selection.objects = new Object[0];
		
		foreach (AudioClip audioclip in audioclips) {
			
			string path = AssetDatabase.GetAssetPath(audioclip);
			
			AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
			
			if(audioImporter.forceToMono != enabled)
			{
				audioImporter.forceToMono = enabled;
				AssetDatabase.ImportAsset(path);
			}
		}
		
	}
	
	static void SetCompressionSettings( AudioCompressionFormat newFormat ) {
		
		
		
		Object[] audioclips = GetSelectedAudioclips();
		
		if(deselectAfterCompletion) Selection.objects = new Object[0];
		
		foreach (AudioClip audioclip in audioclips) {
			
			string path = AssetDatabase.GetAssetPath(audioclip);
			
			AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
			
			if(audioImporter.defaultSampleSettings.compressionFormat != newFormat)
			{
				var sampleSettings = audioImporter.defaultSampleSettings;
				sampleSettings.compressionFormat = newFormat;
				audioImporter.defaultSampleSettings = sampleSettings;
				AssetDatabase.ImportAsset(path);
			}
		}
		
	}

	static void SetLoadInBg( bool enabled )
	{
		Object[] audioclips = GetSelectedAudioclips();
		
		if(deselectAfterCompletion) Selection.objects = new Object[0];
		
		foreach (AudioClip audioclip in audioclips) {
			
			string path = AssetDatabase.GetAssetPath(audioclip);
			
			AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
			
			if(audioImporter.loadInBackground != enabled)
			{
				audioImporter.loadInBackground = enabled;
				AssetDatabase.ImportAsset(path);
			}
		}
	}

	static void SetPreloadAudioData( bool enabled )
	{
		Object[] audioclips = GetSelectedAudioclips();
		
		if(deselectAfterCompletion) Selection.objects = new Object[0];
		
		foreach (AudioClip audioclip in audioclips) {
			
			string path = AssetDatabase.GetAssetPath(audioclip);
			
			AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
			
			if(audioImporter.preloadAudioData != enabled)
			{
				audioImporter.preloadAudioData = enabled;
				AssetDatabase.ImportAsset(path);
			}
		}
	}
	
	
	
	/*static void SelectedSetCompressionBitrate(float newCompressionBitrate) {
		
		
		
		Object[] audioclips = GetSelectedAudioclips();
		
		if(deselectAfterCompletion) Selection.objects = new Object[0];
		
		foreach (AudioClip audioclip in audioclips) {
			
			string path = AssetDatabase.GetAssetPath(audioclip);
			
			AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
			
			if(audioImporter.compressionBitrate != (int)newCompressionBitrate)
			{
				audioImporter.compressionBitrate = (int)newCompressionBitrate;
				AssetDatabase.ImportAsset(path);
			}
		}
		
	}*/
	
	
	
	static void SelectedToggleDecompressOnLoadSettings(AudioClipLoadType enabled) {
		
		
		
		Object[] audioclips = GetSelectedAudioclips();
		
		if(deselectAfterCompletion) Selection.objects = new Object[0];
		
		foreach (AudioClip audioclip in audioclips) {
			
			string path = AssetDatabase.GetAssetPath(audioclip);
			
			AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
			
			if(audioImporter.defaultSampleSettings.loadType != enabled)
			{
				var sampleSettings = audioImporter.defaultSampleSettings;
				sampleSettings.loadType = enabled;
				audioImporter.defaultSampleSettings = sampleSettings;
				AssetDatabase.ImportAsset(path);
			}
		}
		
	}
	
	
	
	/*static void SelectedToggle3DSoundSettings(bool enabled) {
		
		
		
		Object[] audioclips = GetSelectedAudioclips();
		
		if(deselectAfterCompletion) Selection.objects = new Object[0];
		
		foreach (AudioClip audioclip in audioclips) {
			
			string path = AssetDatabase.GetAssetPath(audioclip);
			
			AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
			
			if(audioImporter.threeD != enabled)
			{
				audioImporter.threeD = enabled;
				AssetDatabase.ImportAsset(path);
			}
			
		}
		
	}*/

	
	/*static void SelectedGaplessLoopingSettings(bool enabled) {
		
		
		
		Object[] audioclips = GetSelectedAudioclips();
		
		Selection.objects = new Object[0];
		
		foreach (AudioClip audioclip in audioclips) {
			
			string path = AssetDatabase.GetAssetPath(audioclip);
			
			AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
			
			if(audioImporter.loopable != enabled)
			{
				audioImporter.loopable = enabled;
				AssetDatabase.ImportAsset(path);
			}
		}
		
	}*/
	
	
	
	static Object[] GetSelectedAudioclips()
		
	{
		
		return Selection.GetFiltered(typeof(AudioClip), SelectionMode.DeepAssets);
		
	}
	
}