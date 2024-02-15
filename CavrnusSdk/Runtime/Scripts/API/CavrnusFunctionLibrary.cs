using Collab.Base.Collections;
using Collab.Proxy.Comm.LocalTypes;
using System;
using System.Collections.Generic;
using UnityEngine;
using CavrnusCore;
using CavrnusSdk.Setup;
using UnityBase;
using System.IO;
using System.Threading.Tasks;

namespace CavrnusSdk.API
{
	public static class CavrnusFunctionLibrary
	{
        //Sets up all static helpers and systems required for Cavrnus to run
        public static void InitializeCavrnus()
		{
			CavrnusStatics.Setup(CavrnusSpatialConnector.Instance.AdditionalSettings);
		}

		#region Authentication

		//Checks if you are logged in
		public static bool IsLoggedIn()
		{
			return CavrnusAuthHelpers.CurrentAuthentication != null;
		}

        //Gets user credentials, allowing you to join valid spaces and make other requests
        public static void AuthenticateWithPassword(string server, string email, string password, Action<CavrnusAuthentication> onSuccess, Action<string> onFailure)
		{
			CavrnusAuthHelpers.Authenticate(server.Trim(), email.Trim(), password.Trim(), onSuccess, onFailure);
		}

        //Creates a guest user account with a given name and joins as that user
        public static void AuthenticateAsGuest(string server, string userName, Action<CavrnusAuthentication> onSuccess, Action<string> onFailure)
		{
			CavrnusAuthHelpers.AuthenticateAsGuest(server.Trim(), userName.Trim(), onSuccess, onFailure);
		}

		//Throws an even when user authentication is complete
		public static void AwaitAuthentication(Action<CavrnusAuthentication> onAuth)
		{
			CavrnusAuthHelpers.AwaitAuthentication(onAuth);
		}

		#endregion

		#region Spaces

		//Gets a list of all current spaces which can be joined
		public static void FetchJoinableSpaces(Action<List<CavrnusSpaceInfo>> onRecvCurrentJoinableSpaces)
		{
			CavrnusSpaceHelpers.GetCurrentlyAvailableSpaces(onRecvCurrentJoinableSpaces);
		}

        //Triggers when spaces become available to you that you can join, or when their metadata changes
        public static IDisposable BindJoinableSpaces(Action<CavrnusSpaceInfo> spaceAdded, Action<CavrnusSpaceInfo> spaceUpdated, Action<CavrnusSpaceInfo> spaceRemoved)
		{
			//TODO: FIX SPACEUPDATED SO IT GETS HIT!  (Or have space data in properties...)
			return CavrnusSpaceHelpers.BindAllAvailableSpaces(spaceAdded, spaceUpdated, spaceRemoved);
		}

        //Checks if there is any active connection to a space
        public static bool IsConnectedToAnySpace()
		{
			return CavrnusSpaceHelpers.SpaceConnections.Count > 0;
		}

        //Connects to a Space; joining voice & video and receiving/processing the journal
        public static void JoinSpace(string spaceId, Action<CavrnusSpaceConnection> onConnected, Action<string> onFailure)
		{
			CavrnusSpaceHelpers.JoinSpace(spaceId.Trim(), CavrnusSpatialConnector.Instance.SpawnableObjects, onConnected, onFailure);
		}

        //Triggers when you begin attempting to join a space, returning the ID of the space being joined
        public static void AwaitAnySpaceBeginLoading(Action<string> onBeginLoading)
        {
            CavrnusSpaceHelpers.AwaitAnySpaceBeginLoading(onBeginLoading);
        }

        //Triggers immediately if you are already in a space, otherwise triggers as soon as you connect
        public static void AwaitAnySpaceConnection(Action<CavrnusSpaceConnection> onConnected)
		{
			CavrnusSpaceHelpers.AwaitAnySpaceConnection(onConnected);
		}

        //Disconnects you from a given space.  You will stop receiving property updates, and lose user & voice connections
        public static void ExitSpace(this CavrnusSpaceConnection spaceConn)
		{
			CavrnusSpaceHelpers.SpaceConnections.Remove(spaceConn);
			spaceConn.Dispose();
		}

        #endregion

        #region Properties

        // ============================================
        // Color Property Functions
        // ============================================

        //Defines what the application will show if a new prop value has not been assigned
        public static void DefineColorPropertyDefaultValue(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName, Color propertyValue)
		{
			CavrnusPropertyHelpers.DefineColorPropertyDefaultValue(spaceConn, containerName, propertyName, propertyValue);
		}

        //Gets the current property value, whether the default or the one currently set
        public static Color GetColorPropertyValue(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName)
		{
			return CavrnusPropertyHelpers.GetColorPropertyValue(spaceConn, containerName, propertyName);
		}

        //Triggers an event when the property changes, plus an initial event when first bound.
        public static IDisposable BindColorPropertyValue(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName, Action<Color> onPropertyUpdated)
		{
			return CavrnusPropertyHelpers.BindToColorProperty(spaceConn, containerName, propertyName, onPropertyUpdated);
		}

        //Begins a temporary property update.  This can be updated with UpdateWithNewData() This will show for everyone in the space, but will not be saved unless you call Finish().
        public static CavrnusLivePropertyUpdate<Color> BeginTransientColorPropertyUpdate(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName, Color propertyValue)
		{
			return CavrnusPropertyHelpers.BeginContinuousPropertyUpdate(spaceConn, containerName, propertyName, propertyValue);
		}

        //Updates the property value at the given path and synchronizes the data to the server
        public static void PostColorPropertyUpdate(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName, Color propertyValue)
		{
			CavrnusPropertyHelpers.UpdateColorProperty(spaceConn, containerName, propertyName, propertyValue);
		}

        // ============================================
        // Float Property Functions
        // ============================================

        //Defines what the application will show if a new prop value has not been assigned
        public static void DefineFloatPropertyDefaultValue(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName, float propertyValue)
		{
			CavrnusPropertyHelpers.DefineFloatPropertyDefaultValue(spaceConn, containerName, propertyName, propertyValue);
		}

        //Gets the current property value, whether the default or the one currently set
        public static float GetFloatPropertyValue(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName)
		{
			return CavrnusPropertyHelpers.GetFloatPropertyValue(spaceConn, containerName, propertyName);
		}

        //Triggers an event when the property changes, plus an initial event when first bound.
        public static IDisposable BindFloatPropertyValue(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName, Action<float> onPropertyUpdated)
		{
			return CavrnusPropertyHelpers.BindToFloatProperty(spaceConn, containerName, propertyName, onPropertyUpdated);
		}

        //Begins a temporary property update.  This can be updated with UpdateWithNewData() This will show for everyone in the space, but will not be saved unless you call Finish().
        public static CavrnusLivePropertyUpdate<float> BeginTransientFloatPropertyUpdate(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName, float propertyValue)
		{
			return CavrnusPropertyHelpers.BeginContinuousPropertyUpdate(spaceConn, containerName, propertyName, propertyValue);
		}

        //Updates the property value at the given path and synchronizes the data to the server
        public static void PostFloatPropertyUpdate(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName, float propertyValue)
		{
			CavrnusPropertyHelpers.UpdateFloatProperty(spaceConn, containerName, propertyName, propertyValue);
		}

        // ============================================
        // Bool Property Functions
        // ============================================

        //Defines what the application will show if a new prop value has not been assigned
        public static void DefineBoolPropertyDefaultValue(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName, bool propertyValue)
		{
			CavrnusPropertyHelpers.DefineBooleanPropertyDefaultValue(spaceConn, containerName, propertyName, propertyValue);
		}

        //Gets the current property value, whether the default or the one currently set
        public static bool GetBoolPropertyValue(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName)
		{
			return CavrnusPropertyHelpers.GetBooleanPropertyValue(spaceConn, containerName, propertyName);
		}

        //Triggers an event when the property changes, plus an initial event when first bound.
        public static IDisposable BindBoolPropertyValue(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName, Action<bool> onPropertyUpdated)
		{
			return CavrnusPropertyHelpers.BindToBooleanProperty(spaceConn, containerName, propertyName, onPropertyUpdated);
		}

        //Begins a temporary property update.  This can be updated with UpdateWithNewData() This will show for everyone in the space, but will not be saved unless you call Finish().
        public static CavrnusLivePropertyUpdate<bool> BeginTransientBoolPropertyUpdate(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName, bool propertyValue)
		{
			return CavrnusPropertyHelpers.BeginContinuousPropertyUpdate(spaceConn, containerName, propertyName, propertyValue);
		}

        //Updates the property value at the given path and synchronizes the data to the server
        public static void PostBoolPropertyUpdate(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName, bool propertyValue)
		{
			CavrnusPropertyHelpers.UpdateBooleanProperty(spaceConn, containerName, propertyName, propertyValue);
		}

        // ============================================
        // String Property Functions
        // ============================================

        //Defines what the application will show if a new prop value has not been assigned
        public static void DefineStringPropertyDefaultValue(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName, string propertyValue)
		{
			CavrnusPropertyHelpers.DefineStringPropertyDefaultValue(spaceConn, containerName, propertyName, propertyValue);
		}

        //Gets the current property value, whether the default or the one currently set
        public static string GetStringPropertyValue(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName)
		{
			return CavrnusPropertyHelpers.GetStringPropertyValue(spaceConn, containerName, propertyName);
		}

        //Triggers an event when the property changes, plus an initial event when first bound.
        public static IDisposable BindStringPropertyValue(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName, Action<string> onPropertyUpdated)
		{
			return CavrnusPropertyHelpers.BindToStringProperty(spaceConn, containerName, propertyName, onPropertyUpdated);
		}

        //Begins a temporary property update.  This can be updated with UpdateWithNewData() This will show for everyone in the space, but will not be saved unless you call Finish().
        public static CavrnusLivePropertyUpdate<string> BeginTransientStringPropertyUpdate(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName, string propertyValue)
		{
			return CavrnusPropertyHelpers.BeginContinuousPropertyUpdate(spaceConn, containerName, propertyName, propertyValue);
		}

        //Updates the property value at the given path and synchronizes the data to the server
        public static void PostStringPropertyUpdate(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName, string propertyValue)
		{
			CavrnusPropertyHelpers.UpdateStringProperty(spaceConn, containerName, propertyName, propertyValue);
		}

        // ============================================
        // Vector Property Functions
        // ============================================

		//Defines what the application will show if a new prop value has not been assigned
        public static void DefineVectorPropertyDefaultValue(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName, Vector4 propertyValue)
		{
			CavrnusPropertyHelpers.DefineVectorPropertyDefaultValue(spaceConn, containerName, propertyName, propertyValue);
		}

        //Gets the current property value, whether the default or the one currently set
        public static Vector4 GetVectorPropertyValue(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName)
		{
			return CavrnusPropertyHelpers.GetVectorPropertyValue(spaceConn, containerName, propertyName);
		}

        //Triggers an event when the property changes, plus an initial event when first bound.
        public static IDisposable BindVectorPropertyValue(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName, Action<Vector4> onPropertyUpdated)
		{
			return CavrnusPropertyHelpers.BindToVectorProperty(spaceConn, containerName, propertyName, onPropertyUpdated);
		}

        //Begins a temporary property update.  This can be updated with UpdateWithNewData() This will show for everyone in the space, but will not be saved unless you call Finish().
        public static CavrnusLivePropertyUpdate<Vector4> BeginTransientVectorPropertyUpdate(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName, Vector4 propertyValue)
		{
			return CavrnusPropertyHelpers.BeginContinuousPropertyUpdate(spaceConn, containerName, propertyName, propertyValue);
		}

        //Updates the property value at the given path and synchronizes the data to the server
        public static void PostVectorPropertyUpdate(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName, Vector4 propertyValue)
		{
			CavrnusPropertyHelpers.UpdateVectorProperty(spaceConn, containerName, propertyName, propertyValue);
		}

        // ============================================
        // Transform Property Functions
        // ============================================

        //Defines what the application will show if a new prop value has not been assigned
        public static void DefineTransformPropertyDefaultValue(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName, CavrnusTransformData propertyValue)
		{
			CavrnusPropertyHelpers.DefineTransformPropertyDefaultValue(spaceConn, containerName, propertyName, propertyValue.LocalPosition, propertyValue.LocalEulerAngles, propertyValue.LocalScale);
		}

        //Gets the current property value, whether the default or the one currently set
        public static CavrnusTransformData GetTransformPropertyValue(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName)
		{
			return CavrnusPropertyHelpers.GetTransformPropertyValue(spaceConn, containerName, propertyName);
		}

        //Triggers an event when the property changes, plus an initial event when first bound.
        public static IDisposable BindTransformPropertyValue(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName, Action<CavrnusTransformData> onPropertyUpdated)
		{
			return CavrnusPropertyHelpers.BindToTransformProperty(spaceConn, containerName, propertyName, onPropertyUpdated);
		}

        //Begins a temporary property update.  This can be updated with UpdateWithNewData() This will show for everyone in the space, but will not be saved unless you call Finish().
        public static CavrnusLivePropertyUpdate<CavrnusTransformData> BeginTransientTransformPropertyUpdate(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName, CavrnusTransformData propertyValue)
		{
			return CavrnusPropertyHelpers.BeginContinuousPropertyUpdate(spaceConn, containerName, propertyName, propertyValue);
		}

        //Updates the property value at the given path and synchronizes the data to the server
        public static void PostTransformPropertyUpdate(this CavrnusSpaceConnection spaceConn, string containerName, string propertyName, CavrnusTransformData propertyValue)
		{
			CavrnusPropertyHelpers.UpdateTransformProperty(spaceConn, containerName, propertyName, propertyValue.LocalPosition, propertyValue.LocalEulerAngles, propertyValue.LocalScale);
		}

        #endregion

        #region Permissions
		// ============================================
        // Permissions Functions
        // ============================================

        //Binds an event to throw when a policy is/isn't allowed for the user (returns false until policies are fetched & resolved)
        public static IDisposable BindGlobalPolicy(string policy, Action<bool> onValueChanged)
		{
			return RoleAndPermissionHelpers.EvaluateGlobalPolicy(policy, onValueChanged);
		}

        //Binds an event to throw when a policy is/isn't allowed for the user in a given space (returns false until policies are fetched & resolved)
        public static IDisposable BindSpacePolicy(this CavrnusSpaceConnection conn, string policy, Action<bool> onValueChanged)
		{
			return RoleAndPermissionHelpers.EvaluateSpacePolicy(policy, conn, onValueChanged);
		}

        #endregion

        #region SpawnedObjects
        // ============================================
        // Spawned Objects
        // ============================================

        //Instantiates the given object with no set properties (note you will need to pull the Container ID out of the Spawned Object and assign property values to it)
        public static string SpawnObject(this CavrnusSpaceConnection spaceConn, string uniqueIdentifier)
		{
			var newId = spaceConn.RoomSystem.Comm.CreateNewUniqueObjectId();
			var creatorId = spaceConn.RoomSystem.Comm.LocalCommUser.Value.ConnectionId;
			var contentType = new ContentTypeWellKnownId(uniqueIdentifier);

			var createOp = new OpCreateObjectLive(null, newId, creatorId, contentType).ToOp();

			spaceConn.RoomSystem.Comm.SendJournalEntry(createOp, null);

			return newId;
		}

        //Destroys the given object
        public static void DestroyObject(this CavrnusSpawnedObject spawnedObject)
		{
			OperationIdLive rootOpId = new OperationIdLive(spawnedObject.CreationOpId);

			var singles = new List<string> { rootOpId.Id };

			var deleteOp = new OpRemoveOpsLive(OpRemoveOpsLive.RemovalTypes.None) { OpsToRemove = singles };

            spawnedObject.spaceConnection.RoomSystem.Comm.SendJournalEntry(deleteOp.ToOp(), null);
		}

		#endregion

		#region Space Users

		//Throws an event when the local CavrnusUser arrives in the space
		public static async void AwaitLocalUser(this CavrnusSpaceConnection spaceConnection, Action<CavrnusUser> localUserArrived)
		{
			var lu = await spaceConnection.RoomSystem.AwaitLocalUser();

			localUserArrived(new CavrnusUser(lu, spaceConnection));
		}

        //Gives the list of current users in a space
        public static List<CavrnusUser> GetCurrentSpaceUsers(this CavrnusSpaceConnection spaceConn)
        {
            List<CavrnusUser> res = new List<CavrnusUser>();
            foreach (var user in spaceConn.RoomSystem.Comm.ConnectedUsers)
            {
                res.Add(new CavrnusUser(user, spaceConn));
            }

            return res;
        }

        //Triggers whenever users join or leave a given space
        public static IDisposable BindSpaceUsers(this CavrnusSpaceConnection spaceConn, Action<CavrnusUser> userAdded, Action<CavrnusUser> userRemoved)
		{
			return CavrnusSpaceUserHelpers.BindSpaceUsers(spaceConn, userAdded, userRemoved);
		}

		//Throws an event with the user's current stream image
        public static IDisposable BindUserVideoFrames(this CavrnusUser user, Action<TextureWithUVs> userFrameArrived)
        {
            return user.vidProvider.providedTexture.Bind(frame =>
			{
                Debug.Log($"BUILDING SPRITE FOR USER {user.ContainerId} local({user.IsLocalUser}) with Frame: {frame}");

                if (frame == null)
					return;

                userFrameArrived(frame);
			});
        }

        #endregion

        #region Voice and Video

        //Sets muted state for local user
        public static void SetLocalUserMutedState(this CavrnusSpaceConnection spaceConnection, bool muted)
		{
			spaceConnection.RoomSystem.Comm.LocalCommUser.Value.Rtc.Muted.Value = muted;
		}

        //Sets streaming state for local user
        public static void SetLocalUserStreamingState(this CavrnusSpaceConnection spaceConnection, bool streaming)
		{
			spaceConnection.RoomSystem.Comm.LocalCommUser.Value.UpdateLocalUserCameraStreamState(streaming);
		}

		//Gets the current audio input device
		public static CavrnusInputDevice GetCurrentAudioInputDevice()
		{
			var input = CavrnusStatics.RtcContext.CurrentAudioInputSource.Value;
			return new CavrnusInputDevice(input.Name, input.Id);
		}

        //Gets available microphones
        public static void FetchAudioInputs(Action<List<CavrnusInputDevice>> onRecvDevices)
		{
			CavrnusStatics.RtcContext.FetchAudioInputOptions(res => 
			{
				List<CavrnusInputDevice> devices = new List<CavrnusInputDevice>();
				foreach (var device in res)
				{
					devices.Add(new CavrnusInputDevice(device.Name, device.Id));
				}
				CavrnusStatics.Scheduler.ExecInMainThread(() => onRecvDevices?.Invoke(devices));
			});
		}

        //Sets which microphone to use
        public static void UpdateAudioInput(CavrnusInputDevice device)
		{
			CavrnusStatics.RtcContext.ChangeAudioInputDevice(new Collab.RtcCommon.RtcInputSource() { Id = device.Id, Name = device.Name },
																   (s) => Debug.Log("Changed audio input device to: " + s),
																   err => Debug.LogError("Failed to change audio input device: " + err));
		}

		public static CavrnusVideoInputDevice GetCurrentVideoInputDevice()
		{
			var input = CavrnusStatics.RtcContext.CurrentVideoInputSource.Value;
			return new CavrnusVideoInputDevice(input.Name, input.Id);
		}

        //Gets available camera/stream sources
        public static void FetchVideoInputs(Action<List<CavrnusVideoInputDevice>> onRecvDevices)
		{
			CavrnusStatics.RtcContext.FetchVideoInputOptions(res =>
			{
				List<CavrnusVideoInputDevice> devices = new List<CavrnusVideoInputDevice>();
				foreach (var device in res)
				{
					devices.Add(new CavrnusVideoInputDevice(device.Name, device.Id));
				}
				CavrnusStatics.Scheduler.ExecInMainThread(() => onRecvDevices?.Invoke(devices));
			});
		}

        //Sets which camera/stream source to use
        public static void UpdateVideoInput(CavrnusVideoInputDevice device)
		{
			CavrnusStatics.RtcContext.ChangeVideoInputDevice(new Collab.RtcCommon.RtcInputSource() { Id = device.Id, Name = device.Name },
																   (s) => Debug.Log("Changed video input device to: " + s),
																   err => Debug.LogError("Failed to change video input device: " + err));
		}

		#endregion

		#region Remote Content

		public static void FetchFileById(CavrnusSpaceConnection spaceConn, string id, Func<Stream, long, Task> onStreamLoaded)
		{
			CavrnusContentHelpers.FetchFileById(spaceConn, id, onStreamLoaded);
		}

		public static void FetchAllUploadedContent(Action<List<CavrnusRemoteContent>> onCurrentContentArrived)
		{
			CavrnusContentHelpers.FetchAllUploadedContent(onCurrentContentArrived);
		}

		#endregion
	}
}