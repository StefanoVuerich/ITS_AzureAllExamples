using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dottor.Models;
using Models;
using Dottor.Blobs;

namespace Dottor.PortaleLe.Controllers
{
    public class BlobsController : ApiController
    {
        MyBlob _rep;
        // GET: api/Blobs
        public string Get()
        {
            _rep = new MyBlob();
            var x = _rep.GetAll();
            return x;
        }

        // GET: api/Blobs/5
        public BlobDTO Get(string filename)
        {
            _rep = new MyBlob();
            if (filename != "")
            {
                string extension = filename.Substring(filename.LastIndexOf(".") + 1);
                return (BlobDTO)_rep.Get(filename);
            }
            return null;
        }

        // POST: api/Blobs
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Blobs/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Blobs/5
        public void Delete(int id)
        {
        }
    }
}
