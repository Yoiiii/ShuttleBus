// miniprogram/pages/start/start.js
const util = require('../../utils/util.js');
const config = require('../../config');
const app = getApp()
const request = require('../../utils/request');
import Toast from 'vant-weapp/toast/toast';
Page({
  data: {
    language: ""
  },
  onLoad: function(options) {
    var that = this
    if (options.carID) {
      app.globalData.carID = options.carID
      console.log("CarID", options)
    }
    // if (wx.getStorageSync("language") == "en") {
    //   Toast.loading({
    //     message: 'loading...'
    //   });
    // } else {
    //   Toast.loading({
    //     message: '加載中...'
    //   });
    // }
    // that.setLanguage()
    app.login().then(function (res) {
      if (res.sopenid) {
        that.jump(wx.getStorageSync('sopenid'))
      } else { }
    });
  },
  // setLanguage() {
  //   var that = this
  //   wx.request({
  //     url: `${config.baseUrl}/WeChatWS.asmx/GetLangauge`,
  //     data: {
  //       sopenid: wx.getStorageSync('sopenid'),
  //       langauge: app.globalData.language
  //     },
  //     method: "post",
  //     success(res) {
  //       console.log("[webserver][GetLangauge]:", res.data)
  //       var languages = {}
  //       for (var i = 0; i < res.data.length; i++) {
  //         var tag = res.data[i].TagID
  //         languages[tag] = res.data[i].TagValue
  //       }
  //       console.log("languages", languages)
  //       wx.setStorageSync("languages", languages)
  //       console.log("wx.getStorageSync(languages)", wx.getStorageSync("languages"))
  //       that.setData({
  //         languages: languages
  //       });
  //     }
  //   })
  // },
  jump(sopenid) {
    var that = this
    /*wx.showToast({
      title: '加載中...',
      mask: true,
      icon: 'loading'
    })*/
    let data={
      sopenid: wx.getStorageSync('sopenid')
    }
    request.getUserInfo(data).then(res=>{
      if (res.data.result == "null") {
        wx.redirectTo({
          url: '/pages/binding/binding',
        })
      } else if (res.data.userid) {
        app.globalData.userId = res.data.userid
        app.globalData.userName = res.data.name
        app.globalData.userType = res.data.type
        if (res.data.type == "driver") {
          app.globalData.list = [{
            "pagePath": "/pages/missionInfo/missionInfo",
            "iconPath": "/image/icon_home.png",
            "selectedIconPath": "/image/icon_home_HL.png",
            "text": "當前任務"
          },
          {
            "pagePath": "/pages/missionList/missionList",
            "iconPath": "/image/icon_component.png",
            "selectedIconPath": "/image/icon_component_HL.png",
            "text": "任務列表"
          }
          ]
          wx.switchTab({
            url: '/pages/missionInfo/missionInfo',
          })
        } else if (res.data.type == "admin") {
          app.globalData.list = [{
            "pagePath": "/pages/passenger/passenger",
            "iconPath": "/image/icon_home.png",
            "selectedIconPath": "/image/icon_home_HL.png",
            "text": "當前行程"//that.data.language.currentTrip 
          },
          {
            "pagePath": "/pages/passengerList/passengerList",
            "iconPath": "/image/icon_component.png",
            "selectedIconPath": "/image/icon_component_HL.png",
            "text": "行程列表"//that.data.language.tripList 
          },
          {
            "pagePath": "/pages/list/list",
            "iconPath": "/image/icon_API.png",
            "selectedIconPath": "/image/icon_API_HL.png",
            "text": "任務管理"
          }
          ]
          wx.switchTab({
            url: '/pages/passenger/passenger',
          })
        } else {
          app.globalData.list = [{
            "pagePath": "/pages/passenger/passenger",
            "iconPath": "/image/icon_home.png",
            "selectedIconPath": "/image/icon_home_HL.png",
            "text": "當前行程" //that.data.language.currentTrip
          },
          {
            "pagePath": "/pages/passengerList/passengerList",
            "iconPath": "/image/icon_component.png",
            "selectedIconPath": "/image/icon_component_HL.png",
            "text": "行程列表"//that.data.language.tripList 
          },
          ]
          wx.switchTab({
            url: '/pages/passenger/passenger',
          })
        }
      } else {
        wx.showModal({
          title: '加載失敗',
          content: '請檢查網絡並重試,錯誤代碼:' + res,
          confirmText: "確定",
          showCancel: false,
          success(res) {
            if (res.confirm) {
              that.loadtravelInfo()
              console.log('用户点击确定')
            } else if (res.cancel) {
              console.log('用户点击取消')
            }
          }
        })
      }
    });
    // wx.request({
    //   url: `${config.baseUrl}/WeChatWS.asmx/GetUserInfo`,
    //   data: {
    //     sopenid: wx.getStorageSync('sopenid')
    //   },
    //   method: "post",
    //   success(res) {

    //     console.log("[webserver][GetUserInfo]:", res.data)
    //     if (res.data.result == "null") {
    //       wx.redirectTo({
    //         url: '/pages/binding/binding',
    //       })
    //     } else if (res.data.userid) {
    //       app.globalData.userId = res.data.userid
    //       app.globalData.userName = res.data.name
    //       app.globalData.userType = res.data.type
    //       if (res.data.type == "driver") {
    //         app.globalData.list = [{
    //             "pagePath": "/pages/missionInfo/missionInfo",
    //             "iconPath": "/image/icon_home.png",
    //             "selectedIconPath": "/image/icon_home_HL.png",
    //             "text": "當前任務"
    //           },
    //           {
    //             "pagePath": "/pages/missionList/missionList",
    //             "iconPath": "/image/icon_component.png",
    //             "selectedIconPath": "/image/icon_component_HL.png",
    //             "text": "任務列表"
    //           }
    //         ]
    //         wx.switchTab({
    //           url: '/pages/missionInfo/missionInfo',
    //         })
    //       } else if (res.data.type == "admin") {
    //         app.globalData.list = [{
    //             "pagePath": "/pages/passenger/passenger",
    //             "iconPath": "/image/icon_home.png",
    //             "selectedIconPath": "/image/icon_home_HL.png",
    //           "text": "當前行程"//that.data.language.currentTrip 
    //           },
    //           {
    //             "pagePath": "/pages/passengerList/passengerList",
    //             "iconPath": "/image/icon_component.png",
    //             "selectedIconPath": "/image/icon_component_HL.png",
    //             "text": "行程列表"//that.data.language.tripList 
    //           },
    //           {
    //             "pagePath": "/pages/list/list",
    //             "iconPath": "/image/icon_API.png",
    //             "selectedIconPath": "/image/icon_API_HL.png",
    //             "text": "任務管理"
    //           }
    //         ]
    //         wx.switchTab({
    //           url: '/pages/passenger/passenger',
    //         })
    //       } else {
    //         app.globalData.list = [{
    //             "pagePath": "/pages/passenger/passenger",
    //             "iconPath": "/image/icon_home.png",
    //             "selectedIconPath": "/image/icon_home_HL.png",
    //           "text": "當前行程" //that.data.language.currentTrip
    //           },
    //           {
    //             "pagePath": "/pages/passengerList/passengerList",
    //             "iconPath": "/image/icon_component.png",
    //             "selectedIconPath": "/image/icon_component_HL.png",
    //             "text": "行程列表"//that.data.language.tripList 
    //           },
    //         ]
    //         wx.switchTab({
    //           url: '/pages/passenger/passenger',
    //         })
    //       }
    //     } else {
    //       wx.showModal({
    //         title: '加載失敗',
    //         content: '請檢查網絡並重試,錯誤代碼:' + res,
    //         confirmText: "確定",
    //         showCancel: false,
    //         success(res) {
    //           if (res.confirm) {
    //             that.loadtravelInfo()
    //             console.log('用户点击确定')
    //           } else if (res.cancel) {
    //             console.log('用户点击取消')
    //           }
    //         }
    //       })
    //     }
    //   },
    //   fail(err) {
    //     console.log("[webserver][GetUserInfoFail]:", err)
    //     wx.showModal({
    //       title: '加載失敗',
    //       content: '請檢查網絡並重試,錯誤代碼:' + err,
    //       confirmText: "確定",
    //       showCancel: false,
    //       success(res) {
    //         if (res.confirm) {
    //           that.loadtravelInfo()
    //           console.log('用户点击确定')
    //         } else if (res.cancel) {
    //           console.log('用户点击取消')
    //         }
    //       }
    //     })
    //   }
    // })
  }
})