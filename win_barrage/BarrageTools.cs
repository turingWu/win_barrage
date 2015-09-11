using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Configuration;

namespace win_barrage
{
    class BarrageTools
    {
        private int DATALENGTH = 30;
        private int activityId = 0;
        private string timeStart = null;
        private string domainName = null;
        private string initApiUrl = null;
        private string fetchApiUrl = null;
        public BarrageTools(int activityId)
        {
            this.activityId = activityId;
            string isDebug = ConfigurationManager.AppSettings["debug"];
            if (isDebug.Equals("true"))
            {
                this.domainName = "http://ty.weixiao.qq.com/";
                this.initApiUrl = "http://ty.weixiao.qq.com/activity/get_walled_contents/";
                this.fetchApiUrl = "http://ty.weixiao.qq.com/activity/get_walling_content/";
            }
            else
            {
                this.domainName = "http://weixiao.qq.com/";
                this.initApiUrl = "http://weixiao.qq.com/activity/get_walled_contents/";
                this.fetchApiUrl = "http://weixiao.qq.com/activity/get_walling_content/";
            }
            this.DATALENGTH = Int32.Parse(ConfigurationManager.AppSettings["data_length"]);
        }
        public List<Barrage> initBarrage()
        {
            string apiUrl = initApiUrl;
            apiUrl += activityId;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl + "?length=" + DATALENGTH);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            request.CookieContainer = CookieTools.GetUriCookieContainer(new Uri(domainName));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string responseString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            JObject jsonObj = JsonConvert.DeserializeObject(responseString) as JObject;
            timeStart = jsonObj["time_start"].ToString();
            return JsonConvert.DeserializeObject<List<Barrage>>(jsonObj["contents"].ToString());
        }

        public List<Barrage> fetchNewBarrage()
        {
            string apiUrl = fetchApiUrl;
            apiUrl += activityId;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl + "?length=" + DATALENGTH + "&time_start=" + timeStart);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            request.CookieContainer = CookieTools.GetUriCookieContainer(new Uri(domainName));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string responseString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            JObject jsonObj = JsonConvert.DeserializeObject(responseString) as JObject;
            timeStart = jsonObj["time_start"].ToString();
            return JsonConvert.DeserializeObject<List<Barrage>>(jsonObj["contents"].ToString());
        }
    }
}
