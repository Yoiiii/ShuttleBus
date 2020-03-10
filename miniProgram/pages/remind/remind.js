// pages/remind/remind.js
var util = require('../../utils/util.js');
const config = require('../../config');
const request = require('../../utils/request.js');
Page({
  data: {

  },
  onLoad: function(options) {
    var that = this
    let data={
        sopenid: wx.getStorageSync('sopenid')
    }
    request.getUserIfo(data).then(res=>{
        if (res.data.type == "driver") {
            that.getMission();
        } else if (res.data.type == "passenger") {
            that.getTravel();
        }
    })
  },
  getMission: function() {
    var that = this
    let data={
        sopenid: wx.getStorageSync('sopenid'),
    }
    request.getMission(data).then(res=>{
        if (res.data.result != "null") {
            var time = util.formatonlyMDTime(res.data.time)
            that.setData({
                RID: res.data.RID,
                carID: res.data.carID,
                location: res.data.location,
                destination: res.data.destination,
                name: res.data.username,
                //nowlocation: res.data.name1,
                passengers: res.data.passengers,
                roadlist: res.data.roadlist,
                roads: res.data.roadlist,
                status: res.data.status,
                tel: res.data.tel,
                time: res.data.time,
                hint: res.data.name,
                result: true,
                title: "任務",
                timetype: "開始時間",
                buttontext: "確認任務",
            })
        }
    })
  },
  getTravel: function() {
    var that = this
    let data={
        sopenid: wx.getStorageSync('sopenid'),
    }
    requset.getTravel(data).then(res=>{
        if (res.data.result == "null") {
            that.setData({
                result: false
            })
        } else {
            that.setData({
                RID: res.data.RID,
                PlanRID: res.data.PlanRID,
                carID: res.data.carID,
                location: res.data.location,
                destination: res.data.destination,
                driver: res.data.username,
                time: util.formatonlyMDTime(res.data.time),
                tel: res.data.tel,
                result: true,
                type: "passenger",
                title: "行程",
                timetype: "乘車時間",
                buttontext: "確認行程",
            })
        }
    })
  },
  submit: function(e) {
    var that = this
    console.log(e.detail.formId);
    let data={
        sopenid: wx.getStorageSync('sopenid'),
        fromid: e.detail.formId,
        planRID: that.data.RID,
    }
      request.dirverMessage(data).then(res=>{
          if (res.data.result == "success") {
              wx.showModal({
                  title: '确认成功',
                  content: '将在' + "任務開始" + "前5分鐘發送提示",
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
      })
  },
})