#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2021 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

[UnityEngine.RequireComponent(typeof(AkAudioListener))]
[UnityEngine.DisallowMultipleComponent]
[UnityEngine.DefaultExecutionOrder(-10)]
///@brief Add this script on a listener game object to assign another game object as a distance probe. 
///	The position of the distance probe will be used for distance calculations for the listener, instead of the position of the listener itself. 
///	In third-person perspective applications, the distance probe Game Object may be set to the player character's position, 
///	and the listener Game Object's position to that of the camera. 
///	In this scenario, attenuation is based on the distance between the character and the sound, whereas panning, spatialization, and spread and focus calculations are based on the camera.
public class AkListenerDistanceProbe : UnityEngine.MonoBehaviour
{
	[UnityEngine.Tooltip("Game object that is assigned as the distance probe for this listener.")]
	public AkGameObj distanceProbe;

	private void OnEnable()
	{
        if (distanceProbe)
        {
            var listenerGameObjectID = AkSoundEngine.GetAkGameObjectID(this.gameObject);
            var distanceProbeGameObjectID = AkSoundEngine.GetAkGameObjectID(distanceProbe.gameObject);
			AkSoundEngine.SetDistanceProbe(listenerGameObjectID, distanceProbeGameObjectID);
        }
    }

	private void OnDisable()
	{
        var listenerGameObjectID = AkSoundEngine.GetAkGameObjectID(this.gameObject);
        AkSoundEngine.SetDistanceProbe(listenerGameObjectID, AkSoundEngine.AK_INVALID_GAME_OBJECT);
	}

}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.