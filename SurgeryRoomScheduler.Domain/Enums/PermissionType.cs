using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Enums
{
    public static  class PermissionType
    {

        #region User Controller
        public const string Admin_GetUserList = "5c35db79-2e05-465e-bd1b-680abb5149b2";
        public const string Admin_GetUser = "3e202f54-ba15-4b0b-b7cf-fb9605961f06";
        public const string Admin_AddUser = "4107d207-72fe-4d89-b9c1-df157d5b85b6";
        public const string Admin_DeleteUser = "361bc216-c1da-43eb-8325-1ab1d929edd5";
        public const string Admin_UpdateUser = "56f185b0-fc29-4c6c-b229-2c662c3cb9c2";
        public const string Admin_GetRoleListForUpdateUser = "9c58d7a3-b900-4aaa-829f-90a6d512d927";
        public const string Admin_GetRoleForUpdateUser = "b270cc57-ae6d-48c0-841f-448aa689071a";
        #endregion



        #region  Role Controller
        public const string Admin_GetRolesList = "a228f06f-65b1-4d2b-9508-858f51beb3c9";
        public const string Admin_AddRoles = "2f85dd93-801e-4b8a-8c5e-be2bb64ce6c4";
        public const string Admin_GetRole = "db5afe9a-5ddf-4ee4-b2ee-3580829fa38b";
        public const string Admin_GetPermissions = "aa9ce53d-c6c8-4825-899b-34bf8dcb3206";
        public const string Admin_DeleteRole = "c26fb81e-7ff5-4ed8-91f6-1b88d530a7ab";
        public const string Admin_UpdateRole = "ac3d0146-4998-4941-ae8c-ef881cc6ec4a";
        public const string Admin_GetMenus = "9fe99afa-3f38-4e6e-a77a-743e25898f35";
        #endregion


        #region Timing Controller
        public const string Admin_GetTimingList = "23d8d726-7634-466c-885f-9e8215e3279c";
        public const string Admin_AddTiming = "50dfcbab-d52e-4549-ac78-31bd947b91f7";
        public const string Admin_GetTiming = "2be79773-954b-45d7-935b-b17e94ea4c7c";
        public const string Admin_DeleteTiming = "7e3551d0-97ac-4d4a-b1c2-12502018c735";
        public const string Admin_UpdateTiming = "d0a533ac-e6fb-4282-af4c-9dc7a9709a8d";
        public const string Admin_GetDoctorsList = "1e9e14e1-5024-4882-8e19-4817efa91a4a";
        public const string Admin_GetRoomsList = "454751da-ad53-484b-82a5-3cd6581af497";
        public const string Admin_GetTimingCalender = "ed03c98c-50b3-457e-a814-88c41077bbfb";
        public const string Admin_GetDoctorsRoom = "48bc7aa7-127e-4b25-9198-85416c14e471";
        #endregion


        #region Reservation Controller
        public const string AddReservation = "288bceeb-abc9-48d1-8e48-a1853b1fc47a";
        public const string GetReservationCalender = "d7bbad41-9a6a-45d8-981f-72e6aa6582eb";
        public const string GetDoctorReservedList = "5a289a46-3246-4987-8f7d-e4bece680fca";
        #endregion


        #region Reservation Managment
        public const string Admin_GetReservations = "9ff7cda7-b52e-4e47-b77d-9eb1ab72e62b";
        #endregion

    }
}
