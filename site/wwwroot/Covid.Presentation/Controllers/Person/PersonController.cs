using Covid.Core.Common;
using Covid.Core.DBEntities.Person;
using Covid.Core.DBEntities.UserDetails;
using Covid.Core.IRepo;
using Covid.Infrastructure.Helpers;
using Covid.Infrastructure.ViewModel.Person;
using Covid.Presentation.Common;
using Covid.Presentation.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Covid.Presentation.Controllers.Person
{

    public class PersonController : Controller
    {
        private readonly IPersonRepository dPerson;
        private readonly ICommonRepository dCommon;
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        public PersonController(IPersonRepository dPerson, ICommonRepository dCommon)
        {
            this.dPerson = dPerson;
            this.dCommon = dCommon;
        }
        public ActionResult OpenCovidPersonForm(long Id)
        {
            vmCovidPerson vm = new vmCovidPerson();
            vm.PersonDetail = dPerson.GetCovidPerson(Id);
            // vm.QuarantineDetails= QRepo.GetQuarantineDetails(PersonId);
            return View(Views.PersonDetailUpdateCheckForm, vm);
        }

        public ActionResult UpdateCovidPersonDetails(FormCollection form)
        {
            mCovidPerson PersonDetails = new mCovidPerson();
            PersonDetails.PatientName = form["Name"];
            PersonDetails.Id = Convert.ToInt32(form["PersonId"]);
            PersonDetails.TravelDetails = form["Travel"];
            PersonDetails.WorkDetails = form["Work"];
            PersonDetails.FamilyMemberCuount = Convert.ToInt32( form["FamilyMemberCuount"]);
            PersonDetails.FiftyPlus = Convert.ToInt32(form["FiftyPlus"]);

            PersonDetails.AnyIssueFiftyPlus = form["AnyIssueFiftyPlus"];
            PersonDetails.RoomCount = Convert.ToInt32(form["RoomCount"]);
            PersonDetails.Symtoms = form["Symtoms"];

            PersonDetails.IsSymtoms = form["Fever"] == "on" ? true : false;
            PersonDetails.IsVaccine = form["Cough"] == "on" ? true : false;
            PersonDetails.DoesCount = Convert.ToInt32(form["DoesCount"]);
            PersonDetails.CurrentStatus = Convert.ToInt32(form["Quarantine"]);
            PersonDetails.Comments = form["Remarks"];
            PersonDetails.UpdatedBy = SessionHelper.UserDetails == null ? 0 : SessionHelper.UserDetails.UserId;
            dPerson.UpdateCovidPerson(PersonDetails);
            return Content("OK");
            
        }
        [SessionExpire]
        public ActionResult PersonCreation()
        {
            if (Request.QueryString.Count > 1)
            {
                MobileNumber = Request.QueryString["MobileNo"];
                Password = Request.QueryString["Password"];
                long MobileNo = Convert.ToInt64(MobileNumber);
                string Passwords = Convert.ToString(Password);
                mUserDetails user = dCommon.GetUserDetailByMobileNo(MobileNo, Passwords);
                if (user == null)
                {
                    return View(Views.LoginAPIError);
                }
                SessionHelper.UserDetails = user;
                var ust = dCommon.GetUserTypeByUserTypeId(user.UserTypeId);
                if (ust != null)
                {
                    SessionHelper.UserDetails.UserType = ust.UserType;
                }
            }
            vmPerson vmPersonData = new vmPerson();
            vmPersonData.InfectionSourceList = dCommon.GetInfectionSource();
            vmPersonData.LocalBodyList = dCommon.GetLocalyBody();
            return View(Views.PersonCreation, vmPersonData);
        }
        public ActionResult GetPersonCreation(vmPerson vmPersonData, string Positive, string HealthStatus, string Quarantine, string Sticker, int? InfectionSource, int? LocalBodyId, int? ZoneId, int? WardId, string Suspect)
        {
            mPerson Person = new mPerson();
            int Mode = 1;
            Person = vmPersonData.PersonDetail;

            if (Person.PersonId == 0)
            {
                Mode = 1;
                //var m = dPerson.GetPersonExistByNameandMobile(Person.PersonName, Person.Mobileno);
                //if (m != null)
                //{
                //    TempData["PersonMessage"] = "Data Already Exists";
                //    return RedirectToAction("PersonCreation");
                //}
            }
            else
            {
                Mode = 2;
            }
            Person.IsPositive = Convert.ToInt32(Positive);
            Person.HealthStatus = Convert.ToInt32(HealthStatus);
            Person.Isquarantine = Convert.ToInt32(Quarantine);
            Person.IsSticker = Convert.ToInt32(Sticker);
            Person.InfectionSource = InfectionSource;
            Person.LocalBodyId = Convert.ToInt32(LocalBodyId);
            Person.WardId = Convert.ToInt32(WardId);
            Person.ZoneId = Convert.ToInt32(ZoneId);
            Person.CreatedBy = SessionHelper.UserDetails.Name;
            Person.UpdatedBy = SessionHelper.UserDetails.Name;
            Person.IsSuspect = Convert.ToInt32(Suspect);
            int PersonID = dPerson.AddPerson(Person, Mode);
            if (PersonID == 0)
                TempData["PersonMessage"] = "Something Went Wrong..";

            if (Mode == 1)
                TempData["PersonMessage"] = "Data Submit Sucessfully with PersonID - " + PersonID + ".";

            if (Mode == 2)
                TempData["PersonMessage"] = "Data Updated Sucessfully with PersonID - " + PersonID + ".";
            if (SessionHelper.UserDetails.UserType.ToUpper() == UserTypeCommon.RRT.ToUpper())
            {
                return Redirect("/DashBoard/GetIPersonDeailByFilter?Isquarantine=0&IsPositive=0");
            }
            return RedirectToAction("PersonCreation");
        }
        public ActionResult GetPersonDetail(int PersonId)
        {
            vmPerson vmPersonData = new vmPerson();
            vmPersonData.PersonDetail = dPerson.GePerson(PersonId);
            vmPersonData.InfectionSourceList = dCommon.GetInfectionSource();
            vmPersonData.LocalBodyList = dCommon.GetLocalyBody();
            vmPersonData.GetZoneList = dCommon.GetZoneOrWardByParentID(vmPersonData.PersonDetail.LocalBodyId);
            vmPersonData.GetWardList = dCommon.GetZoneOrWardByParentID(vmPersonData.PersonDetail.ZoneId);


            return View(Views.PersonCreation, vmPersonData);
        }
        [HttpGet]
        public JsonResult FetchZone(int ID)
        {
            var data = dCommon.GetZoneOrWardByParentID(ID).Select(l => new { Value = l.LocalBodyID, Text = l.LocalBodyName });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FetchWard(int ID)
        {

            var data = dCommon.GetZoneOrWardByParentID(ID).Select(l => new { Value = l.LocalBodyID, Text = l.LocalBodyName });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CovidPersonSearch()
        {
            vmCovidPerson vmPersonData = new vmCovidPerson();
            vmPersonData.LocalBodyList = dCommon.GetLocalyBody();
            vmPersonData.GetRefGroupList = dPerson.GetRefGroupDetail();
            return View(Views.CovidPersonSearch, vmPersonData);
        }
            [SessionExpire]
        public ActionResult PersonSearch()
        {
            if (Request.QueryString.Count > 1)
            {
                MobileNumber = Request.QueryString["MobileNo"];
                Password = Request.QueryString["Password"];
                long MobileNo = Convert.ToInt64(MobileNumber);
                string Passwords = Convert.ToString(Password);
                mUserDetails user = dCommon.GetUserDetailByMobileNo(MobileNo, Passwords);
                if (user == null)
                {
                    return View(Views.LoginAPIError);
                }
                SessionHelper.UserDetails = user;
                var ust = dCommon.GetUserTypeByUserTypeId(user.UserTypeId);
                if (ust != null)
                {
                    SessionHelper.UserDetails.UserType = ust.UserType;
                }
            }
            vmPerson vmPersonData = new vmPerson();
            vmPersonData.InfectionSourceList = dCommon.GetInfectionSource();
            vmPersonData.LocalBodyList = dCommon.GetLocalyBody();
            vmPersonData.CheckPointList = dCommon.GetCheckPoint();
            vmPersonData.PositivePersonDetailList = dPerson.GetPositivePersonDetail();
            vmPersonData.CurrentLocationDetailList = dPerson.GetCurrenTLocationDetail();
            return View(Views.PersonSearch, vmPersonData);
        }
        public ActionResult GetIPersonDeailByFilter(string LOcalBodyID, string ZoneID, string WardID, string DateOfArrival, string Quarantine, string HealthStatus,
            string TravalMode,string PersonName,string ComingFrom, string InfectionSource, string Sticker, int? RelatedPersonID, int? CheckPoint, string Sick, string Fever, string ShortnessofBreath,string CurrentLocation)
        {
            vmPerson vmPersonData = new vmPerson();
            vmPersonData.InfectionSourceList = dCommon.GetInfectionSource();
            vmPersonData.LocalBodyList = dCommon.GetLocalyBody();
            vmPersonData.PersonAllDetalsList = dPerson.GetIPersonDeailByFilter(LOcalBodyID, ZoneID, WardID, DateOfArrival, Quarantine, HealthStatus, TravalMode,PersonName,ComingFrom, InfectionSource, Sticker, RelatedPersonID, CheckPoint, Sick, Fever, ShortnessofBreath,CurrentLocation);
            return PartialView(Views.PersonSearchPartial, vmPersonData);
        }

        public ActionResult GetCovidPersonDeailByFilter(string LocalBodyID, string ZoneID, string WardID, string Quarantine, string RelatedPersonID)
        {
            vmCovidPerson vmPersonData = new vmCovidPerson();
            vmPersonData.LocalBodyList = dCommon.GetLocalyBody();
            vmPersonData.CovidPersonDetailList = dPerson.GetCovidPersonDeailByFilter(LocalBodyID, ZoneID, WardID, Quarantine, RelatedPersonID);
            vmPersonData.GetRefGroupList = dPerson.GetRefGroupDetail();
            return PartialView(Views.CovidPersonSearchPartial, vmPersonData);
        }
        [SessionExpire]
        public ActionResult CheckPostPersonCreation()
        {
            if (Request.QueryString.Count > 1)
            {
                MobileNumber = Request.QueryString["MobileNo"];
                Password = Request.QueryString["Password"];
                long MobileNo = Convert.ToInt64(MobileNumber);
                string Passwords = Convert.ToString(Password);
                mUserDetails user = dCommon.GetUserDetailByMobileNo(MobileNo, Passwords);
                if (user == null)
                {
                    return View(Views.LoginAPIError);
                }
                SessionHelper.UserDetails = user;
                var ust = dCommon.GetUserTypeByUserTypeId(user.UserTypeId);
                if (ust != null)
                {
                    SessionHelper.UserDetails.UserType = ust.UserType;
                }
            }
            vmPerson vmPersonData = new vmPerson();
            vmPersonData.LocalBodyList = dCommon.GetLocalyBody();
            vmPersonData.CheckPointList = dCommon.GetCheckPoint();
            return View(vmPersonData);
        }

        public ActionResult NewEntryPersonCreation()
        {
            vmPerson vmPersonData = new vmPerson();
            return View(vmPersonData);
        }
        public ActionResult SubmitCheckPostPersonCreation(FormCollection form)
        {

            mPerson Person = new mPerson();
            Person.PersonName = Convert.ToString(form["PersonName"]).Trim();
            Person.Dateofarrival = Convert.ToDateTime(form["Dateofarrival"]);
            Person.ComingFrom = Convert.ToString(form["ComingFrom"]);
            Person.GoingTo = Convert.ToString(form["GoingTo"]);
            Person.IsPermissionPass = Convert.ToInt32(form["IsPermissionPass"]);
            Person.Address = Convert.ToString(form["Address"]);
            Person.PermanentAddress = Convert.ToString(form["PAddress"]);
            Person.Mobileno = Convert.ToString(form["Mobileno"]);
            Person.Email = Convert.ToString(form["Email"]);
            if (form["LocalBody"] != null)
            {
                Person.LocalBodyId = Convert.ToInt32(form["LocalBody"]);
                if (!String.IsNullOrEmpty(form["Ward"].ToString()))
                {
                    Person.WardId = Convert.ToInt32(form["Ward"]);
                    Person.ZoneId = dCommon.GetWardParentid(Person.WardId).LocalBodyParentId;
                }
            }

            Person.TwoMonthsDetails = Convert.ToString(form["TwoMonthsDetails"]);
            Person.FifteenDaysDetails = Convert.ToString(form["FifteenDaysDetails"]);
            Person.IsCough = Convert.ToInt32(form["IsCough"]);
            Person.IsFever = Convert.ToInt32(form["IsFever"]);
            Person.IsShortnessofBreath = Convert.ToInt32(form["IsShortnessofBreath"]);
            Person.CoronaRemark = Convert.ToString(form["CoronaRemark"]);
            Person.CheckpostId = Convert.ToInt32(form["CheckPoint"]);
            Person.TravelMode = "ByRoad";


            Person.CreatedBy = SessionHelper.UserDetails.Name;

            //var m = dPerson.GetPersonExistByNameandMobile(Person.PersonName, Person.Mobileno);
            //if (m != null)
            //{
            //    TempData["PersonMessage"] = "Data Already Exists";
            //    return RedirectToAction("CheckPostPersonCreation");
            //}
            int PersonID = dPerson.AddPerson(Person, 1);
            TempData["PersonMessage"] = "Data Submit Sucessfully with PersonID - " + PersonID + ".";

            if (PersonID == 0)
                TempData["PersonMessage"] = "Something Went Wrong..";

            return RedirectToAction("CheckPostPersonCreation");
        }

        public ActionResult SubmitNewEntryPersonCreation(FormCollection form)
        {

            mOutSiderForm Person = new mOutSiderForm();
            Person.Name = Convert.ToString(form["Name"]).Trim();
            Person.Gender = Convert.ToString(form["Gender"]);
            Person.ComingFromOtherState = Convert.ToString(form["ComingFromOtherState"]);
            Person.GoingToOtherState = Convert.ToString(form["GoingToOtherState"]);
            Person.ComingAddressofOtherState = Convert.ToString(form["ComingAddressofOtherState"]);
            Person.GoingAddressofOtherState = Convert.ToString(form["GoingAddressofOtherState"]);
            Person.AddressofJabalpur = Convert.ToString(form["AddressofJabalpur"]);
            Person.MobileNumber = Convert.ToInt64(form["MobileNumber"]);
            Person.Occupation = Convert.ToString(form["Occupation"]);

            Person.NoofPersons = Convert.ToInt32(form["NoofPersons"]);
           

            int PersonID = dPerson.AddOutSiderPerson(Person, 1);
            TempData["PersonMessage"] = "Data Submit Sucessfully with PersonID - " + PersonID + ".";

            if (PersonID == 0)
                TempData["PersonMessage"] = "Something Went Wrong..";

            return RedirectToAction("NewEntryPersonCreation");
        }
        [SessionExpire]
        public ActionResult GetNewEntryPersonDetails()
        {
            if (Request.QueryString.Count > 1)
            {
                MobileNumber = Request.QueryString["MobileNo"];
                Password = Request.QueryString["Password"];
                long MobileNo = Convert.ToInt64(MobileNumber);
                string Passwords = Convert.ToString(Password);
                mUserDetails user = dCommon.GetUserDetailByMobileNo(MobileNo, Passwords);
                if (user == null)
                {
                    return View(Views.LoginAPIError);
                }
                SessionHelper.UserDetails = user;
                var ust = dCommon.GetUserTypeByUserTypeId(user.UserTypeId);
                if (ust != null)
                {
                    SessionHelper.UserDetails.UserType = ust.UserType;
                }
            }
            vmPerson vm = new vmPerson();
            return View(Views.NewEntryPersonDetails, vm);
        }
        public ActionResult SearchGetNewEntryDetails(string SelectedDate)
        {
            DateTime? StartDate, EndDate;
            vmPerson vm = new vmPerson();
            if (string.IsNullOrEmpty(SelectedDate))
            {
                StartDate = null;
                EndDate = null;
            }
            else
            {
                string[] Idates = SelectedDate.Split('-');
                StartDate = Convert.ToDateTime(Idates[0]);
                EndDate = Convert.ToDateTime(Idates[1]);
            }
            // var QPL = QRepo.GetQuarantinePersonByPersonId(StartDate, EndDate).Select(x => new { x.PersonId, x.PersonName, x.MoileNo, x.Address });
            vm.OutSiderFormList = dPerson.GetNewEntryPersondetailByDate(StartDate, EndDate);
            if (vm.OutSiderFormList == null)
            {
                TempData["PersonMessage"] = "No Search Found!!";
            }
            return PartialView(Views.NewEntryPersonDetailsPartial, vm);
        }

       [SessionExpire]
        public ActionResult PersondetailsHierarichal()
        {
            if (Request.QueryString.Count > 1)
            {
                MobileNumber = Request.QueryString["MobileNo"];
                Password = Request.QueryString["Password"];
                long MobileNo = Convert.ToInt64(MobileNumber);
                string Passwords = Convert.ToString(Password);
                mUserDetails user = dCommon.GetUserDetailByMobileNo(MobileNo, Passwords);
                if (user == null)
                {
                    return View(Views.LoginAPIError);
                }
                SessionHelper.UserDetails = user;
                var ust = dCommon.GetUserTypeByUserTypeId(user.UserTypeId);
                if (ust != null)
                {
                    SessionHelper.UserDetails.UserType = ust.UserType;
                }
            }
                vmPerson vm = new vmPerson();
            List<PersonTreeView> P = new List<PersonTreeView>();
            //P = GetPersonTree(0,ref P);
            //vm.PList = P;
            vm.map = 0;
            return View(Views.PersondetailsHierarichal, vm);
        }
        public ActionResult PersondetailsHierarichalMap()
        {
            vmPerson vm = new vmPerson();
            List<PersonTreeView> P = new List<PersonTreeView>();
            ViewBag.Markers = "";
            vm.map = 1;
            return View(Views.PersondetailsHierarichal, vm);
        }
        public ActionResult GetPartialPersonHierchyBySearch(string Search, int map)
        {
            vmPerson vm = new vmPerson();
            var View = string.Empty;
            List<PersonTreeView> P = new List<PersonTreeView>();
            vm.map = map;
            string markers = "[";
            var s = dPerson.GetAlldetailsPersonBySearch(Search);
            if (map == 0)
            {

                if (s.Count() != 0)
                {
                    for (int i = 0; i < s.Count(); i++)
                    {
                        for (int j = 0; j < s.Count(); j++)
                        {
                            if (s[i].PersonId == s[j].PersonRelationID)
                            {
                                s.Remove(s[j]);
                            }
                        }


                    }
                    foreach (var i in s)
                    {
                       
                            P = dPerson.GetAlldetailsPersonforTree(i.PersonId);
                            P = GetPersonTree(i.PersonId, ref P);
                            vm.PList.AddRange(P);
                       
                    }

                }
                else
                {
                    TempData["PersonMessage"] = "No Search Found!!";
                }

                View = Views.PersondetailsHierarichalPartial;
            }
            else if (map == 1)
            {
                if (s.Count != 0)
                {

                    for(int i=0;i<s.Count();i++)
                    {
                        for (int j = 0; j < s.Count(); j++)
                        {
                            if (s[i].PersonId == s[j].PersonRelationID)
                            {
                                s.Remove(s[j]);
                            }
                        }
                            
                        
                    }

                    foreach (var i in s)
                    {

                        P = dPerson.GetAlldetailsPersonforTree(i.PersonId);
                        P = GetPersonTree(i.PersonId, ref P);
                        vm.PList.AddRange(P);

                    }

                    markers += "{";
                    markers += string.Format("'title': '{0}',", "JABALPUR");
                    markers += string.Format("'lat': '{0}',", "23.1815");
                    markers += string.Format("'lng': '{0}',", "79.9864");
                    markers += string.Format("'description': '{0}'", "Testing");
                    markers += "},";


                    markers += "];";
                    ViewBag.Markers = markers;
                }
                else
                {
                    TempData["PersonMessage"] = "No Search Found!!";
                }

                View = Views.PersondetailsHierarichalPartialMap;
            }
            return PartialView(View, vm.PList);
        }
        public List<PersonTreeView> GetPersonTree(long relationid, ref List<PersonTreeView> RP)
        {

            foreach (var i in RP)
            {
                var r = new List<PersonTreeView>();

                i.List = dPerson.GeRelatedPerson(i.PersonId);
                r = i.List;
                i.List = GetPersonTree(i.PersonId, ref r);

            }

            return RP;
        }

        public ActionResult GetDataforPartialView(long PersonId)
        {
            vmPerson person = new vmPerson();
            person.PersonAllDetail = dPerson.GetAlldetailsPerson(PersonId);

            return PartialView(Views.PersonDetailsModal, person);

        }
        public ActionResult GetQuarDataforPartialView(long PersonId)
        {
            vmPerson person = new vmPerson();
            person.QList = dPerson.GetQuarantinePersonByPersonId(PersonId);

            return PartialView(Views.GetQuarantineDetailsPersonPartial, person);

        }
       [SessionExpire]
        public ActionResult PesronWithNoWard()
        {
            if (Request.QueryString.Count > 1)
            {
                MobileNumber = Request.QueryString["MobileNo"];
                Password = Request.QueryString["Password"];
                long MobileNo = Convert.ToInt64(MobileNumber);
                string Passwords = Convert.ToString(Password);
                mUserDetails user = dCommon.GetUserDetailByMobileNo(MobileNo, Passwords);
                if (user == null)
                {
                    return View(Views.LoginAPIError);
                }
                SessionHelper.UserDetails = user;
                var ust = dCommon.GetUserTypeByUserTypeId(user.UserTypeId);
                if (ust != null)
                {
                    SessionHelper.UserDetails.UserType = ust.UserType;
                }
            }
            vmPerson vmPersonData = new vmPerson();
            vmPersonData.PersonAllDetalsList = dPerson.GetAllpersonwithoutWard(null);
            vmPersonData.LocalBodyList = dCommon.GetLocalyBody();
            vmPersonData.PositivePersonDetailList = dPerson.GetPositivePersonDetail();
            return View(vmPersonData);

        }
        public ActionResult PersonWithNoWardSearchbyPositive(string RelatedPersonID)
        {
            long? RPersonID = RelatedPersonID == "" ? (long?)null: Convert.ToInt64(RelatedPersonID);
            vmPerson vmPersonData = new vmPerson();
            vmPersonData.LocalBodyList = dCommon.GetLocalyBody();
            vmPersonData.PersonAllDetalsList = dPerson.GetAllpersonwithoutWard(RPersonID);


            return PartialView(Views.PersonWithNoWardPartial,vmPersonData);
        }
        public ActionResult PersonWithNoWardUpdate(long PersonId,int? wards,int? LocalBodyId)
        {
          
           
            var ZoneId = dCommon.GetWardParentid(wards).LocalBodyParentId;
             dPerson.UpdateWardIdLocalBodyIdZoneIdPerson(wards,LocalBodyId,ZoneId,PersonId);
           
            TempData["PersonMessage"] = "Data Updated Sucessfully with PersonID - " + PersonId + ".";
           
          
            return RedirectToAction("PesronWithNoWard");
        }
        public JsonResult GetWardDropDown(int LocalBodyId)
        {
            var WardList = dCommon.GetWardByParentID(LocalBodyId).Select(l => new { Value = l.LocalBodyID, Text = l.LocalBodyName });
            return Json(WardList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult HealthUpdate()
        {
            
            vmPerson p = new vmPerson();
            p.PersonAllDetail = null;
            return View(Views.HealthUpdate, p);
        }
        public ActionResult HealthUpdateBySearch(string Search)
        {
           
            vmPerson P = new vmPerson();
            P.PersonAllDetalsList = dPerson.GetAlldetailsPersonBySearch(Search);
            if (P.PersonAllDetalsList.Count()==0)
            {
                TempData["PersonMessage"] = "No Search Found!!";
            }
            return PartialView(Views.HealthUpdatePartial, P);
        }
    }
}