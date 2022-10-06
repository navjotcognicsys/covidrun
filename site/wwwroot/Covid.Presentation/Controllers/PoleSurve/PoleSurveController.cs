using Covid.Core.DBEntities.PoleSurve;
using Covid.Core.IRepo;
using Covid.Presentation.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Covid.Core.Enum;
using Covid.Presentation.Common;
using Covid.Infrastructure.Helpers;
using Covid.Core.Common;
using Covid.Core.DBEntities.LocalBody;
using Covid.Core.DBEntities.UserDetails;
using Covid.Infrastructure.ViewModel.PoleSurve;
using System.Web.SessionState;

namespace Covid.Presentation.Controllers.PoleSurve
{

    public class PoleSurveController : Controller
    {
        IPoleSurveRepository cRepo;
        ICommonRepository commonRepo;

        public PoleSurveController(IPoleSurveRepository ICRepo, ICommonRepository common)
        {
            this.commonRepo = common;
            this.cRepo = ICRepo;
        }

        [SessionExpire]
        public ActionResult StreetLightSurve()
        {
            vmPoleSurve vm = new vmPoleSurve();
            vm.WardList = commonRepo.GetWardByParentID(1); // for Jabalpur only
            return View(Views.StreetLightSurve, vm);
            
        }
        public ActionResult OpenPoleSurveReports()
        {
            vmPoleSurve vm = new vmPoleSurve();
            return View(Views.GetPoleSurveReports, vm);
        }

        [SessionExpire]
        public ActionResult SwitchingSurve()
        {
            vmPoleSurve vm = new vmPoleSurve();
            //mStreetLightSurve StreetSurve = new mStreetLightSurve();
            //StreetSurve.IsCable = true;
            //StreetSurve.AreaType = "Rare";
            //StreetSurve.FittingType = "Test";
            //StreetSurve.FittingVolt = 12;
            //StreetSurve.Latitude = 32.34;
            //StreetSurve.Longitude = 25.4;
            //StreetSurve.MountingHeight = 12;
            //StreetSurve.PoleHeight = 12;
            //StreetSurve.PoleNum = "12";
            //StreetSurve.PoleSpan = 12;
            //StreetSurve.PoleType = "Tesr";
            //StreetSurve.RoadWidth = 12;
            //StreetSurve.TrafficLevel = "Moderate";
            //StreetSurve.UserId = 1;
            //long q = cRepo.InsertStreetLightSurve(StreetSurve);
            vm.WardList = commonRepo.GetWardByParentID(1); // for Jabalpur only
            return View(Views.SwitchingSurve, vm);

        }
        public ActionResult InsertStreetLightSurveAPI( int UserId, string Longitude, string Latitude, bool IsCable, string PoleNum, string FittingType, int FittingVolt, string PoleType, int PoleHeight,
            int RoadWidth, string MountingHeight, int PoleSpan, string AreaType, string TrafficLevel,string PoleImagePath)
        {
            try
            {
                if (Longitude != "" && Latitude != "" )
                {
                    mStreetLightSurve StreetSurve = new mStreetLightSurve();
                    StreetSurve.IsCable = IsCable;
                    StreetSurve.AreaType = AreaType;
                    StreetSurve.FittingType = FittingType;
                    StreetSurve.FittingVolt = FittingVolt;
                    StreetSurve.Latitude = Convert.ToDouble(Latitude);
                    StreetSurve.Longitude = Convert.ToDouble(Longitude);
                    StreetSurve.MountingHeight = MountingHeight;
                    StreetSurve.PoleHeight = PoleHeight;
                    StreetSurve.PoleNum = PoleNum;
                    StreetSurve.PoleSpan = PoleSpan;
                    StreetSurve.PoleType = PoleType;
                    StreetSurve.RoadWidth = RoadWidth;
                    StreetSurve.TrafficLevel = TrafficLevel;
                    StreetSurve.UserId = UserId;
                    StreetSurve.PoleImagePath = PoleImagePath;

                    long q = cRepo.InsertStreetLightSurve(StreetSurve);
                    if (q != 0)
                    {
                        var result = new { Status = "true", Message = "Data Submitted" };
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var result = new { Status = "false", Message = "Problem in data" };
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    var result = new { Status = "false", Message = "No Long Lat Capture" };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var result = new { Status = "false", Message = ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult InsertSwitchingSurveAPI(int UserId, string Longitude, string Latitude, string PointName, string MeterNumber, string IVRS, string ConnectionType, string OperationPhase,
            string MPPKVVCLDivisionName, string MPPKVVCLFeederName, int WardId, string WardName, double VoltageRPhase, double VoltageYPhase, double VoltageBPhase, double CurrentRPhase, double CurrentYPhase,
            double CurrentBPhase, double PowerFactorRPhase, double PowerFactorYPhase, double PowerFactorBPhase, double PowerRPhase, double PowerYPhase, double PowerBPhase, double EnergyRPhase, double EnergyYPhase,
            double EnergyBPhase, double CurrentReading, double MaxDemand, double DailyConsumtion, double MonthlyConsumtion, double YearlyConsumption, string SwitchImagePath)
        {
            try
            {

                if (Longitude != "" && Latitude != "")
                {
                    mSwitchingSurve SwitchSurve = new mSwitchingSurve();
                    SwitchSurve.ConnectionType = ConnectionType;
                    SwitchSurve.CurrentBPhase = CurrentBPhase;
                    SwitchSurve.CurrentReading = CurrentReading;
                    SwitchSurve.CurrentRPhase = CurrentRPhase;
                    SwitchSurve.CurrentYPhase = CurrentYPhase;
                    SwitchSurve.DailyConsumtion = DailyConsumtion;
                    SwitchSurve.EnergyBPhase = EnergyBPhase;
                    SwitchSurve.EnergyRPhase = EnergyRPhase;
                    SwitchSurve.EnergyYPhase = EnergyYPhase;
                    SwitchSurve.IVRS = IVRS;
                    SwitchSurve.Latitude = Convert.ToDouble(Latitude); 
                    SwitchSurve.Longitude = Convert.ToDouble(Longitude); 
                    SwitchSurve.MaxDemand = MaxDemand;
                    SwitchSurve.MeterNumber = MeterNumber;
                    SwitchSurve.MonthlyConsumtion = MonthlyConsumtion;
                    SwitchSurve.MPPKVVCLDivisionName = MPPKVVCLDivisionName;
                    SwitchSurve.MPPKVVCLFeederName = MPPKVVCLFeederName;
                    SwitchSurve.OperationPhase = OperationPhase;
                    SwitchSurve.PointName = PointName;
                    SwitchSurve.PowerBPhase = PowerBPhase;

                    SwitchSurve.PowerFactorBPhase = PowerFactorBPhase;
                    SwitchSurve.PowerFactorRPhase = PowerFactorRPhase;
                    SwitchSurve.PowerFactorYPhase = PowerFactorYPhase;
                    SwitchSurve.PowerRPhase = PowerRPhase;
                    SwitchSurve.PowerYPhase = PowerYPhase;
                    SwitchSurve.UserId = UserId;
                    SwitchSurve.VoltageBPhase = VoltageBPhase;
                    SwitchSurve.VoltageRPhase = VoltageRPhase;
                    SwitchSurve.VoltageYPhase = VoltageYPhase;
                    SwitchSurve.WardId = WardId;
                    SwitchSurve.WardName = WardName;
                    SwitchSurve.YearlyConsumption = YearlyConsumption;
                    SwitchSurve.SwitchImagePath = SwitchImagePath;


                    long q = cRepo.InsertSwitchingSurve(SwitchSurve);
                    if (q != 0)
                    {
                        var result = new { Status = "true", Message = "Data Submitted" };
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var result = new { Status = "false", Message = "Data wrong" };
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var result = new { Status = "false", Message = "No Long Lat Capture" };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var result = new { Status = "false", Message = ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetPoleSurveReports(string SelectedDate)
        {
            DateTime? StartDate, EndDate;
            vmPoleSurve vm = new vmPoleSurve();
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

            vm.StreetLightSurveList = cRepo.GetStreetLightSurveReport(StartDate, EndDate);
            if (vm.StreetLightSurveList == null)
            {
                TempData["QMessage"] = "No Search Found!!";
            }
            return PartialView(Views.GetPoleSurveDetailsPartial, vm);
        }




    }
}