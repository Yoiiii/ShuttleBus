// miniprogram/pages/missionList/missionList.js
var util = require('../../utils/util.js');
const config = require('../../config');
const request = require('../../utils/request.js');
Page({
    /**  
     * 页面的初始数据
     */
    data: {
        driverList: [],
        index: 0,
        carIdList: [],
        index2: 0,
        stime: "",
        etime: "",
        planList: [{
                type: "未完成",
                travelList: []
            },
            {
                type: "全部",
                travelList: []
            }
        ],
    },
    onShow: function() {
        if (typeof this.getTabBar === 'function' &&
            this.getTabBar()) {
            this.getTabBar().setData({
                selected: 2 //这个数是，tabBar从左到右的下标，从0开始
            })
        }

    },
    bindPickerChange: function(e) {
        console.log('picker发送选择改变，携带值为', e.detail.value)
        this.setData({
            index: e.detail.value
        })
    },
    bindPickerChange2: function(e) {
        console.log('picker发送选择改变，携带值为', e.detail.value)
        this.setData({
            index2: e.detail.value
        })
    },
    bindDateChange: function(e) {
        this.setData({
            stime: e.detail.value
        })
    },
    bindDateChange2: function(e) {
        this.setData({
            etime: e.detail.value
        }) 
    },
    onLoad: function() {
        var that = this
        that.loadinfo()
        this.searchFirst();
    },
    loadinfo: function() {
        var that = this
        var stime = util.getNowFormatDate()
        var etime = util.getNowFormatDate()
        that.setData({
            stime: stime,
            etime: etime,
        })
        let data = {
            sopenid: wx.getStorageSync('sopenid')
        }
        request.getAllDriverList(data).then(res => {
            var driverList = ["全部"];
            var driverIdList = ["ALL"];
            for (var i = 0; i < res.data.length; i++) {
                driverList.push(res.data[i].username)
                driverIdList.push(res.data[i].userid)
            }
            that.setData({
                driverList: driverList,
                driverIdList: driverIdList,
            })
        })
        request.getAllCarList(data).then(res => {
            var carIdList = ["全部"];
            for (var i = 0; i < res.data.length; i++) {
                carIdList.push(res.data[i].carID)
            }
            that.setData({
                carIdList: carIdList,
            })
        })
        //that.searchFirst();
    },
    search: function() {
        var that = this
        that.setData({
            'planList[0].travelList': "",
            'planList[1].travelList': "",
        })
        var carID = ""
        if (that.data.index2 == 0) {
            carID = "ALL"
        } else {
            carID = that.data.carIdList[that.data.index2]
        }
        let data={
            sopenid: wx.getStorageSync('sopenid'),
            driverID: that.data.driverIdList[that.data.index],
            CarID: carID,
            stime: that.data.stime,
            etime: that.data.etime,
        }
        request.getALLMissionList(data).then(res=>{
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
                        PlanRID: planList[i].id,
                        carID: planList[i].carID,
                        destination: planList[i].destination,
                        location: planList[i].location,
                        status: planList[i].status,
                        time: util.formatonlyMDTime(planList[i].time),
                        username: planList[i].username,
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
    searchFirst: function() {
        var that = this
        let data={
            sopenid: wx.getStorageSync('sopenid'),
            driverID: "ALL",
            CarID: "ALL",
            stime: that.data.stime,
            etime: that.data.etime,
        }
        request.getALLMissionList(data).then(res => {
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
                        PlanRID: planList[i].id,
                        carID: planList[i].carID,
                        destination: planList[i].destination,
                        location: planList[i].location,
                        status: planList[i].status,
                        time: util.formatonlyMDTime(planList[i].time),
                        username: planList[i].username,
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
    showDetail: function(e) {
        console.log("showd", e)
        var planRID = e.detail.planRID
        wx.navigateTo({
            url: '../detail/detail?planRID=' + planRID,
        })
    },
    jumptoChart: function(e) {
        var planRID = e.currentTarget.dataset.planid
        wx.navigateTo({
            url: '../chat/chat',
        })
    },
    jumptoBar: function(e) {
        var planRID = e.currentTarget.dataset.planid
        wx.navigateTo({
            url: '../bar/bar',
        })
    },
})