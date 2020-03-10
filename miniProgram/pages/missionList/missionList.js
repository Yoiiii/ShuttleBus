// miniprogram/pages/missionList/missionList.js
var util = require('../../utils/util.js');
const config = require('../../config');
const request = require('../../utils/request.js');
Page({
  /**
   * 页面的初始数据
   */
  data: {
    planList: [{
        type: "未完成",
        travelList: [
        ]
      },
      {
        type: "全部",
        travelList: [
        ]
      }
    ],
  },
  /**  
   * 生命周期函数--监听页面加载
   */
  onLoad:function(){
    var that = this
    that.loadinfo()

  },
  onShow: function(options) {
    var that = this
    if (typeof this.getTabBar === 'function' &&
      this.getTabBar()) {
      this.getTabBar().setData({
        selected: 1, //这个数是，tabBar从左到右的下标，从0开始
        list: [
          {
            "pagePath": "../missionInfo/missionInfo",//"pagePath": "../missionList/missionList",
            "iconPath": "../image/icon_home.png",
            "selectedIconPath": "../image/icon_home_HL.png",
            "text": "當前任務"
          },
          {
            "pagePath": "../missionList/missionList",
            "iconPath": "../image/icon_component.png",
            "selectedIconPath": "../image/icon_component_HL.png",
            "text": "任務列表"
          }
        ]
      })}
  },
  onPullDownRefresh: function () {
    var that = this
    wx.showNavigationBarLoading();
    that.loadinfo()
  },
  loadinfo: function() {
    var that = this
    that.setData({
      'planList[0].travelList': "",
      'planList[1].travelList': "",
    })
    let data={
        sopenid: wx.getStorageSync('sopenid')
    }
      request.getMissionList(data).then(res=>{
          if (res.data.result == "null") {
              that.setData({
                  result: "null"
              })
          } else {
              var planList = res.data
              var travelList1 = []
              var travelList2 = []
              var travel = {}
              for (var i = 0; i < planList.length; i++) {
                  travel = {
                      PlanRID: planList[i].PlanRID,
                      carID: planList[i].carID,
                      destination: planList[i].destination,
                      location: planList[i].location,
                      status: planList[i].status,
                      time: util.formatonlyMDTime(planList[i].time),
                  }
                  if (travel.status != 3) {
                      travelList1.push(travel)
                  }
                  travelList2.push(travel)
              }
              that.setData({
                  'planList[0].travelList': travelList1,
                  'planList[1].travelList': travelList2,
              })
          }
          wx.hideNavigationBarLoading();
          wx.stopPullDownRefresh();
      })
  },
  tabChangeIndex(e) {
    //console.log(e.detail.idx)
    this.setData({
      idx: e.detail.idx
    })
  },
})