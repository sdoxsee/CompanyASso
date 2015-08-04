﻿using Sample;
using System;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Thinktecture.IdentityModel.Client;

namespace MvcCodeFlowClientManual.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Request.GetOwinContext().Authentication.SignOut("Cookies");

            return View();
        }

        [HttpPost]
        public ActionResult Index(string scopes)
        {
            var state = Guid.NewGuid().ToString("N");
            var nonce = Guid.NewGuid().ToString("N");
            SetTempState(state, nonce);

            var client = new OAuth2Client(new Uri(Constants.AuthorizeEndpoint));
            
            var url = client.CreateCodeFlowUrl(
                clientId:    "codeclient",
                scope:        scopes,
                redirectUri: "https://localhost:44312/callback",
                state:       state,
                nonce:       nonce);

            return Redirect(url);
        }

        private void SetTempState(string state, string nonce)
        {
            var tempId = new ClaimsIdentity("TempState");
            tempId.AddClaim(new Claim("state", state));
            tempId.AddClaim(new Claim("nonce", nonce));

            Request.GetOwinContext().Authentication.SignIn(tempId);
        }
    }
}