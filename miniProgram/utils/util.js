const config = require('../config');
const formatTime = date => {
  const year = date.getFullYear()
  const month = date.getMonth() + 1
  const day = date.getDate()
  const hour = date.getHours()
  const minute = date.getMinutes()
  const second = date.getSeconds()
  return [year, month, day].map(formatNumber).join('/') + ' ' + [hour, minute, second].map(formatNumber).join(':')
}
const formatonlyTime = date => {
  var y = date.getFullYear();
  var m = date.getMonth() + 1;
  m = m < 10 ? ('0' + m) : m;
  var d = date.getDate();
  d = d < 10 ? ('0' + d) : d;
  var h = date.getHours();
  h = h < 10 ? ('0' + h) : h;
  var minute = date.getMinutes();
  minute = minute < 10 ? ('0' + minute) : minute;
  var second = date.getSeconds();
  second = second < 10 ? ('0' + second) : second;
  return h + ':' + minute + ':' + second;
};
// 时间戳转时间
function transformTime(time = +new Date()) {

  var date = new Date(time + 8 * 3600 * 1000); // 增加8小时

  return date.toJSON().substr(0, 19).replace('T', ' ');

}
function formatonlyMDTime(data) {
  var s = data;
  s = s.replace(/-/g, "/");
  var date2 = new Date(s);
  var week = "日一二三四五六".charAt(date2.getDay());
  var m = date2.getMonth()+1 
  var d = date2.getDate()
  var h = date2.getHours()
  var min = date2.getMinutes()
  if (min.toString().length == 1) {
    min = "0" + min.toString()
  }
  var time = m + "/" + d + " " + h + ":" + min + "(" + week + ")"
  return time;
};

function getDistanceTime(time) {
  var endTime = new Date(Date.parse(time.replace(/-/g, "/"))); /*replace将时间字符串中所有的'-'替换成'/',parse将时间格式的字符串转换成毫秒*/
  var nowTime = new Date();
  var distance = endTime.getTime() - nowTime.getTime(); /*getTime把一个date对象转换成毫秒*/
  var day = 0;
  var hour = 0;
  var minute = 0;
  var second = 0;
  if (distance >= 0) {
    day = Math.floor(distance / 1000 / 60 / 60 / 24);
    hour = Math.floor(distance / 1000 / 60 / 60 % 24);
    minute = Math.floor(distance / 1000 / 60 % 60);
    second = Math.floor(distance / 1000 % 60);
  } else {
    var m = endTime.getMinutes()
    var len = 0;
    for (var i = 0; i < m.toString().length; i++) {
      var c = m.toString().charCodeAt(i);
      //单字节加1 
      if ((c >= 0x0001 && c <= 0x007e) || (0xff60 <= c && c <= 0xff9f)) {
        len++;
      } else {
        len += 2;
      }
    }
    if (len == 1) {
      m = "0" + m
    }
    return endTime.getHours() + ":" + m
  }
  if (day < 1 && hour < 1) {
    return minute + "分鐘"
  } else {
    var d = "";
    var dhour = endTime.getHours()
    if (day >= 1) {
      dhour = dhour + (day * 24) + hour
    }
    if (dhour >= 24 && dhour < 48) {
      d = "明天";
    } else if (dhour < 24) {
      d = ""
    } 
     else if (dhour >= 48 && dhour < 72) {
      d = "後天"
    } else {
      d = endTime.getMonth() + 1 + "/" + endTime.getDate();
    }
    return d + " " + endTime.getHours() + ":" + endTime.getMinutes()
  }
} 

function strlen(str) {
  var len = 0;
  for (var i = 0; i < str.length; i++) {
    var c = str.charCodeAt(i);
    //单字节加1 
    if ((c >= 0x0001 && c <= 0x007e) || (0xff60 <= c && c <= 0xff9f)) {
      len++;
    } else {
      len += 2;
    }
  }
  return len;
}

function formatonlyHMTime(data) {
  var s = data;
  s = s.replace(/-/g, "/");
  var date2 = new Date(s);
  var week = "日一二三四五六".charAt(date2.getDay());
  var m = date2.getMonth() + 1
  var d = date2.getDate()
  var h = date2.getHours()
  var min = date2.getMinutes()
  if (min.toString().length == 1) {
    min = "0" + min.toString()
  }
  var time = h + ":" + min 
  return time;
  /*if (data) {
    var s = data;
    s = s.replace(/-/g, "/");
    var date2 = new Date(s);
    var h = date2.getHours()
    var d = date2.getMinutes()
    var l = 0;
    var a = d;
    for (var i = 0; i < a.length; i++) {
      if (a[i].charCodeAt(0) < 299) {
        l++;
      } else {
        l += 2;
      }
    }
    if (l == 1) {
      d = "0" + d
    }
    var time = h + ":" + d
    return time;
  }*/
};
const formatDateTime = date => {
  var y = date.getFullYear();
  var m = date.getMonth() + 1;
  m = m < 10 ? ('0' + m) : m;
  var d = date.getDate();
  d = d < 10 ? ('0' + d) : d;
  var h = date.getHours();
  h = h < 10 ? ('0' + h) : h;
  var minute = date.getMinutes();
  minute = minute < 10 ? ('0' + minute) : minute;
  var second = date.getSeconds();
  second = second < 10 ? ('0' + second) : second;
  return y + '-' + m + '-' + d + ' ' + h + ':' + minute + ':' + second;
};
//计算两个定位点的距离
function getGreatCircleDistance(lat1, lng1, lat2, lng2) {
  var radLat1 = getRad(lat1);
  var radLat2 = getRad(lat2);
  var a = radLat1 - radLat2;
  var b = getRad(lng1) - getRad(lng2);
  var s = 2 * Math.asin(Math.sqrt(Math.pow(Math.sin(a / 2), 2) + Math.cos(radLat1) * Math.cos(radLat2) * Math.pow(Math.sin(b / 2), 2)));
  s = s * EARTH_RADIUS;
  s = Math.round(s * 10000) / 10000.0;
  return s;
}
var EARTH_RADIUS = 6378137.0; //单位M
var PI = Math.PI;

function getRad(d) {
  return d * PI / 180.0;
}

//調用云函數
const formatNumber = n => {
  n = n.toString()
  return n[1] ? n : '0' + n
}


// 显示繁忙提示
var showBusy = text => wx.showToast({
  title: text,
  icon: 'loading',
  duration: 10000
})

// 显示成功提示
var showSuccess = text => wx.showToast({
  title: text,
  icon: 'success'
})

// 显示失败提示
var showModel = (title, content) => {
  wx.hideToast();
  wx.showModal({
    title,
    content: JSON.stringify(content),
    showCancel: false
  })
}

function formatDate(value) {
  var date = new Date(value).format("yyyy-MM-dd HH:mm");
  if (date == "1970-01-01 08:00")
    return "--";
  else
    return date;
}

function convertUTCTimeToLocalTime(UTCDateString) {
  if (!UTCDateString) {
    return '-';
  }

  function formatFunc(str) { //格式化显示
    return str > 9 ? str : '0' + str
  }
  var date2 = new Date(UTCDateString); //这步是关键
  var year = date2.getFullYear();
  var mon = formatFunc(date2.getMonth() + 1);
  var day = formatFunc(date2.getDate());
  var hour = date2.getHours();
  var min = formatFunc(date2.getMinutes());
  var dateStr = year + '-' + mon + '-' + day + ' ' + hour + ':' + min;
  return dateStr;
}

function login() {
  wx.login({
    success(res) {
      if (res.code) {
        // 发起网络请求
        wx.request({
          url: `${config.baseUrl}/WeChatWS.asmx/login`,
          data: {
            code: res.code
          },
          method: "post",
          success(res) {
            wx.setStorageSync('sopenid', res.data.sopenid)
            console.log("[webserver][login]:", res.data)
          }
        })
      } else {
        console.log('登录失败！' + res.errMsg)
      }
    }
  })
}

function binding(tel, userID) {
  wx.login({
    success(res) {
      if (res.code) {
        // 发起网络请求
        wx.request({
          url: `${config.baseUrl}/WeChatWS.asmx/Binding`,
          data: {
            code: res.code,
            tel: tel,
            userID: userID
          },
          method: "post",
          success(res) {
            console.log("[webserver][Binding]:", res.data)
            return res.data.result
          }
        })
      } else {
        console.log('登录失败！' + res.errMsg)
      }
    }
  })
}
function getNowFormatDate() {
  var date = new Date();
  var seperator1 = "-";
  var year = date.getFullYear();
  var month = date.getMonth() + 1;
  var strDate = date.getDate();
  if (month >= 1 && month <= 9) {
    month = "0" + month;
  }
  if (strDate >= 0 && strDate <= 9) {
    strDate = "0" + strDate;
  }
  var currentdate = year + seperator1 + month + seperator1 + strDate;
  return currentdate;
}
// 翻译站点
function translation(station, language) {
  if (station == "高高") {
    if (language == "en-US") {
      return "GOGO"
    } 
  }
  if (station == "高高後勤") {
    if (language == "en-US") {
      return "GGO"
    } 
  }
  if (station == "高芬") {
    if (language == "en-US") {
      return "GOFUN"
    } 
  }
  if (station == "三惟正") {
    if (language == "en-US") {
      return "3D"
    }
  }
  if (station == "茶山站") {
    if (language == "en-US") {
      return "CHASHAN"
    }
  }
  return station;
};
const formatRoadline = data => {//生成路線圖
  let roadlist = []
  let x = 1
  let roads = {}
  let car = ""
  let road = ""
  let location = ""
  for (let i = 0; i <= data.roadlist.length - 1; i++) {
    if (i == 0) {
      location = data.roadlist[i].location//util.translation(data.roadlist[i].location, that.data.languages)
      if (data.roadlist[0].atime) {
        road = "roads"
        if (data.roadlist[i].ltime == "") {
          car = "car"
        } else {
          car = "nocar"
        }
        roads = {
          no: x,
          car: car,
          road: road,
          location: location,
        }
      } else {
        if (data.status == "0") {
          roads = {
            no: 0,
            car: "nocar",
            road: "roadn",
            location: "",
          }
        } else {
          roads = {
            no: 0,
            car: "car",
            road: "roadn",
            location: "",
          }
        }
        roadlist.push(roads)
        roads = {
          no: x,
          car: "nocar",
          road: "road",
          location: location,
        }
      }
    } else if (i == data.roadlist.length - 1) {
      location = data.roadlist[i].location// util.translation(data.roadlist[i].location, that.data.languages)
      road = "roade"
      if (data.roadlist[i].ltime == "" && data.roadlist[i].atime != "") {
        car = "car"
      } else {
        car = "nocar"
      }
      roads = {
        no: x,
        car: car,
        road: road,
        location: location,
      }
    } else {
      location = data.roadlist[i].location //location = util.translation(data.roadlist[i].location, that.data.languages)
      if (data.roadlist[i].ltime == "" && data.roadlist[i].atime != "") {
        car = "car"
      } else {
        car = "nocar"
      }
      roads = {
        no: x,
        car: car,
        road: "road",
        location: location,
      }
    }
    roadlist.push(roads);
    x++
    if (i != data.roadlist.length - 1) {
      if (data.roadlist[i].ltime != "" && data.roadlist[i + 1].atime == "") {
        car = "car"
      } else {
        car = "nocar"
      }
      roads = {
        no: x,
        car: car,
        road: "roadn",
        location: ""
      }
      roadlist.push(roads);
      x++
    }
  }
  return roadlist;
}

module.exports = {
  formatRoadline,
  getDistanceTime,
  formatTime,
  showBusy,
  showSuccess,
  showModel,
  formatonlyTime,
  formatDateTime,
  getGreatCircleDistance,
  convertUTCTimeToLocalTime,
  login,
  binding,
  formatonlyMDTime,
  formatonlyHMTime,
  getNowFormatDate,
  translation,
}