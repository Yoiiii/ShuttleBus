using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeChatWS.QYWeixin
{
    class Image
    {
        private string _media_id;
        /// <summary>
        /// 要发送的文本内容字段，必须小写，企业微信API不识别大写。
        /// </summary>
        public string media_id
        {
            get { return _media_id; }
            set { _media_id = value; }
        }

    }
    class CorpSendImge : CorpSendBase
    {
        private Image _image;
        /// <summary>
        /// 要发送的文本，必须小写，企业微信API不识别大写。
        /// </summary>
        public Image image
        {
            get { return _image; }
            set { this._image = value; }
        }


        public CorpSendImge(string media_id)
        {
            base.msgtype = "image";
            this.image = new Image
            {
                media_id = media_id
            };
        }
    }
}
