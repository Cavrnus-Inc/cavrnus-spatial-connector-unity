using System.Collections.Generic;
using UnityEngine;
using CavrnusSdk.API;
using CavrnusSdk.PropertySynchronizers;
using CavrnusCore;

namespace CavrnusSdk.Setup
{
	public class CavrnusAvatarManager 
	{
		private GameObject remoteAvatarPrefab;
		private bool showLocalUser;

	    private Dictionary<string, GameObject> avatarInstances = new Dictionary<string, GameObject>();

		public void Setup(GameObject remoteAvatarPrefab, bool showLocalUser)
	    {
			this.remoteAvatarPrefab = remoteAvatarPrefab;
			this.showLocalUser = showLocalUser;

			if (remoteAvatarPrefab == null)
		    {
			    Debug.LogError("No Avatar Prefab has been assigned. Shutting down CoPresence display system.");
				return;
		    }

			CavrnusFunctionLibrary.AwaitAnySpaceConnection(OnSpaceConnection);
		}

		private CavrnusSpaceConnection cavrnusSpaceConnection = null;

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
				user.SpaceConnection.BeginTransientBoolPropertyUpdate(user.ContainerId, "AvatarVis", showLocalUser);
				
				return;
			}

			var initialTransform = user.SpaceConnection.GetTransformPropertyValue(user.ContainerId, "Transform");

			var avatar = Object.Instantiate(remoteAvatarPrefab, initialTransform.Position, Quaternion.Euler(initialTransform.EulerAngles));
            avatar.AddComponent<CavrnusUserFlag>().User = user;
            user.BindUserName(n =>
            {
	            if (avatar != null) 
		            avatar.name = $"{user.ContainerId} ({n}'s Avatar)";
            });
			
			user.SpaceConnection.DefineBoolPropertyDefaultValue(user.ContainerId, "AvatarVis", false);
			user.SpaceConnection.BindBoolPropertyValue(user.ContainerId, "AvatarVis", vis => {
				if (avatar != null)
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

		//Destroy them when we lose that user
		private void UserRemoved(CavrnusUser user)
		{
			if (avatarInstances.ContainsKey(user.ContainerId))
			{
				Object.Destroy(avatarInstances[user.ContainerId].gameObject);
				avatarInstances.Remove(user.ContainerId);
			}
		}
	}
}