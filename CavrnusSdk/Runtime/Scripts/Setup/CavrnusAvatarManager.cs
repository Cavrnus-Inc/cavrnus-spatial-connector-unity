using System;
using System.Collections.Generic;
using UnityEngine;
using CavrnusSdk.API;
using CavrnusSdk.PropertySynchronizers;
using CavrnusCore;
using CavrnusSdk.PropertySynchronizers.CommonImplementations;

namespace CavrnusSdk.Setup
{
	public class CavrnusAvatarManager 
	{
		private GameObject remoteAvatarPrefab;

	    private Dictionary<string, GameObject> avatarInstances = new Dictionary<string, GameObject>();

		// Start is called before the first frame update
		public void Setup(GameObject remoteAvatarPrefab)
	    {
			this.remoteAvatarPrefab = remoteAvatarPrefab;

			if (remoteAvatarPrefab == null)
		    {
			    Debug.LogError("No Avatar Prefab has been assigned.  Shutting down CoPresence display system.");
				return;
		    }

			CavrnusFunctionLibrary.AwaitAnySpaceConnection(OnSpaceConnection);
		}

		private CavrnusSpaceConnection cavrnusSpaceConnection = null;
		private IDisposable hasMovedBind;

		private void OnSpaceConnection(CavrnusSpaceConnection obj)
		{
			cavrnusSpaceConnection = obj;

			cavrnusSpaceConnection.BindSpaceUsers((u) => CavrnusStatics.Scheduler.ExecInMainThreadAfterFrames(3, () => UserAdded(u)), UserRemoved);
		}

		//Instantiate avatars when we get a new user
		private void UserAdded(CavrnusUser user)
		{
			//This list contains the player.  But we don't wanna show their avatar via this system.
			if (user.IsLocalUser) {
				user.SpaceConnection.BeginTransientBoolPropertyUpdate(user.ContainerId, "AvatarVis", true);
				
				return;
			}

			var initialTransform = user.SpaceConnection.GetTransformPropertyValue(user.ContainerId, "Transform");

			var avatar = GameObject.Instantiate(remoteAvatarPrefab, initialTransform.Position, Quaternion.Euler(initialTransform.EulerAngles));
            avatar.AddComponent<CavrnusUserFlag>().User = user;
			avatar.name = $"{user.ContainerId} ({user.GetUserName()}'s Avatar)";
			
			user.SpaceConnection.DefineBoolPropertyDefaultValue(user.ContainerId, "AvatarVis", false);
			hasMovedBind = user.SpaceConnection.BindBoolPropertyValue(user.ContainerId, "AvatarVis", vis => {
				avatar.SetActive(vis);
			});

            CavrnusPropertyHelpers.ResetLiveHierarchyRootName(avatar, $"{user.ContainerId}");

            foreach (var sync in avatar.GetComponentsInChildren<CavrnusValueSync<bool>>())
				sync.SendMyChanges = false;
			foreach (var sync in avatar.GetComponentsInChildren<CavrnusValueSync<float>>())
				sync.SendMyChanges = false;
			foreach (var sync in avatar.GetComponentsInChildren<CavrnusValueSync<Color>>())
				sync.SendMyChanges = false;
			foreach (var sync in avatar.GetComponentsInChildren<CavrnusValueSync<Vector4>>())
				sync.SendMyChanges = false;
			foreach (var sync in avatar.GetComponentsInChildren<CavrnusValueSync<CavrnusTransformData>>())
				sync.SendMyChanges = false;
			foreach (var sync in avatar.GetComponentsInChildren<CavrnusValueSync<string>>())
				sync.SendMyChanges = false;

			avatarInstances[user.ContainerId] = avatar;
		}
		
		private static bool HasTransformChanged(CavrnusTransformData initialTransform, CavrnusTransformData data, float positionThreshold = 0.1f, float rotationThreshold = 0.1f)
		{
			return Vector3.Distance(initialTransform.Position, data.Position) > positionThreshold ||
			       Vector3.Distance(initialTransform.EulerAngles, data.EulerAngles) > rotationThreshold;
		}

		//Destroy them when we lose that user
		private void UserRemoved(CavrnusUser user)
		{
			if (avatarInstances.ContainsKey(user.ContainerId))
			{
				GameObject.Destroy(avatarInstances[user.ContainerId].gameObject);
				avatarInstances.Remove(user.ContainerId);
			}
		}
	}
}