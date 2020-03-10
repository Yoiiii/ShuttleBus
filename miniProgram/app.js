  //app.js
const util = require('utils/util.js');
const request = require('utils/request');
const config = require('config');
App({
  globalData: {
    //isIphoneX:false,
    planId: "",
    userId: "",
    userName: "",
    userType: "",
    carID: "",
    systemInfo: null,
    windowHeight: null, // rpx换算px后的窗口高度
    screenHeight: null, // rpx换算px后的屏幕高度
    list: [
      {
        "pagePath": "../passenger/passenger",
        "iconPath": "../image/icon_home.png",
        "selectedIconPath": "../image/icon_home_HL.png",
        "text": "當前行程"
      },
      {
        "pagePath": "../passengerList/passengerList",
        "iconPath": "../image/icon_component.png",
        "selectedIconPath": "../image/icon_component_HL.png",
        "text": "行程列表"
      },
    ], //存放tabBar的数据
  },
  onLaunch: function() {
    wx.getSystemInfo({
      success: res => {
        let modelmes = res.model;
        //if (modelmes.search('iPhone X') != -1) {
          //that.globalData.isIphoneX = true
        //}
        this.globalData.systemInfo = res
        this.globalData.language = res.language
        console.log("language", res.language)
        if (res.language=="zh"){
          this.globalData.language ="zh-CHT"
        }
        this.globalData.windowHeight = res.windowHeight / (res.windowWidth / 750)
        this.globalData.screenHeight = res.screenHeight / (res.screenWidth / 750)
        console.log("windowHeight", this.globalData.windowHeight)
      }
    })
  },
  login: function () {
    var that = this;
    return new Promise(function (resolve, reject) {
      // 调用登录接口
      wx.login({
        success(res) {
          if (res.code) {
            // 发起网络请求
            let data={
                code: res.code
            }
            request.login(data).then(res=>{
              wx.setStorageSync('sopenid', res.data.sopenid)
              resolve(res.data);
            })
          } else {
            console.log('登录失败！' + res.errMsg)
            reject('登录失败！' + res.errMsg);
          }
        }
      })
    });
  },
})