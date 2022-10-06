using Covid.Core.DBEntities.UserDetails;
using Covid.Core.IRepo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using Covid.Infrastructure.Helpers;

namespace Covid.Presentation.Controllers.CommonAPI
{
    public class CommonAPIController : ApiController
    {
        private readonly IUserRepository userRepo;
        private readonly ICommonRepository commonRepo;
        public CommonAPIController(IUserRepository userRepo, ICommonRepository commonRepo)
        {
            this.userRepo = userRepo;
            this.commonRepo = commonRepo;
        }


       


        //[Route("api/CommonAPI/SubmitUserAttendanceAPI")]
        //[HttpPost]
        //public HttpResponseMessage SubmitUserAttendanceAPI(HttpRequestMessage request)
        //{
        //    try
        //    {
        //        string GUID = null;
        //       mImageFields image = new mImageFields();
                
                

        //        //need to extract data from json..
        //        string json = request.Content.ReadAsStringAsync().Result;

        //        image = JsonConvert.DeserializeObject<mImageFields>(json);
                


        //        if (image != null)
        //        {
        //            var path = "";
        //            string date = DateTime.Now.ToString("yyyyMMdd");
        //            if (!Directory.Exists("~/WebImages/"))
        //            {
        //                Directory.CreateDirectory("~/WebImages/");
        //            }
        //            path = Path.Combine("/WebImages/", image.UserID+"_" + date + "_" + "test.jpg");
        //            byte[] bytes = Convert.FromBase64String(image.ImageByte);
        //            System.IO.File.WriteAllBytes(path, bytes);

        //            string src = "";
        //            var scheme = Request.RequestUri.Scheme; // will get http, https, etc.
        //            var host = Request.RequestUri.Host; // will get www.mywebsite.com
        //            var port = Request.RequestUri.Port; // will get the port
        //            var ip = scheme + "://" + host + ":" + port + "/";
        //            src = ip + "/WebImages/" + image.UserID + "_" + date + "_" + "test.jpg";
        //            int q = userRepo.InsertUserAttendanceByUserIDandDate(image.UserID, Convert.ToDouble(image.Longitude), Convert.ToDouble(image.Latitude), src);
        //            if (q == 1)
        //            {
        //                var result = new { Status = "true", Message = "Attendance done for today" };
        //                return request.CreateResponse(HttpStatusCode.OK, result);
        //            }

        //        }
        //        {

        //            var result = new { Status = "false", Message = "Something went wrong" };
        //            return request.CreateResponse(HttpStatusCode.OK, result);
        //        }
                
               
        //    }
        //    catch (Exception ex)
        //    {
        //        var result = new { Status = "false", Message = "Something went wrong" };
        //        return request.CreateResponse(HttpStatusCode.InternalServerError, result);
        //    }
        //}
    }
}
