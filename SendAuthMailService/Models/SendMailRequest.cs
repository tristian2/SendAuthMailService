/*was using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;*/
//add other from my MSC example
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Web;

namespace SendAuthMailService.Models
{
    public class SendMailRequest
    {
        public SendMailRequest() { }
        public string recipient { get; set; }
        public string cc { get; set; }
        public string replyto { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public string filecontent { get; set; }
        public string filename { get; set; }
    }
}