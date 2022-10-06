using Covid.Core.DBEntities.PTC;
using Covid.Core.IRepo;
using Covid.Presentation.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Covid.Core.Enum;
using Covid.Presentation.Common;
using Covid.Infrastructure.Helpers;
using Covid.Core.Common;
using System.Web.SessionState;
using System.Web.Mvc;

namespace Covid.Presentation.Controllers.PTCGame
{
    public class PTCGameController : Controller
    {
        IPtcGameRepository cRepo;

        public PTCGameController(IPtcGameRepository ICRepo)
        {
            this.cRepo = ICRepo;
        }

        public ActionResult UpadteGameStatusInDB(int GameId, int Status)
        {
            cRepo.UpadteGameStatusInDB(GameId, Status);
            return Json("1", JsonRequestBehavior.AllowGet);
        }

        public ActionResult InertGameInDB(int GameId, int Status)
        {
            cRepo.InertGameInDB(GameId, Status);
            return Json("1", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetGameStatusId(int GameId)
        {
            int GameStaus = cRepo.GetGameStatus(GameId);
            return Json(GameStaus, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetGames(int Status)
        {
            List<mPTC> GameDetailList = new List<mPTC>();
            GameDetailList = cRepo.GetGames(Status);
            return Json(GameDetailList, JsonRequestBehavior.AllowGet);
        }

    }
}