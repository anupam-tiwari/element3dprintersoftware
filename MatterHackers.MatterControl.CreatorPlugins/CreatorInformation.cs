using System;

namespace MatterHackers.MatterControl.CreatorPlugins
{
	public class CreatorInformation
	{
		public delegate void UnlockFunction();

		public delegate bool PermissionFunction();

		public delegate void UnlockRegisterFunction(EventHandler functionToCallOnEvent, ref EventHandler functionThatWillBeCalledToUnregisterEvent);

		public UnlockFunction unlockFunction;

		public PermissionFunction permissionFunction;

		public UnlockRegisterFunction unlockRegisterFunction;

		public Action Show;

		public string iconPath;

		public string description;

		public bool paidAddOnFlag;

		public CreatorInformation(Action showFunction, string iconPath, string description, bool paidAddOnFlag = false, UnlockFunction unlockFunction = null, PermissionFunction permissionFunction = null, UnlockRegisterFunction unlockRegisterFunction = null)
		{
			Show = showFunction;
			this.iconPath = iconPath;
			this.description = description;
			this.paidAddOnFlag = paidAddOnFlag;
			this.unlockFunction = unlockFunction;
			this.permissionFunction = permissionFunction;
			this.unlockRegisterFunction = unlockRegisterFunction;
		}
	}
}
