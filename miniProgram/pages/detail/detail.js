// pages/detail/detail.js
const config = require('../../config');
var util = require('../../utils/util.js');
const request = require('../../utils/request.js');
Page({
    data: {
        planRID: null,
        type:""
    },
    onLoad: function(options) {
        var that = this
        console.log(that.options.planRID)
        that.setData({
            planRID: that.options.planRID
        })
    },
    loadtravelInfo: function(e) {
        let that = this
        let data = {
            sopenid: wx.getStorageSync('sopenid'),
            planID: that.data.planRID
        }
        request.getMissionDetail(data).then(res => {
            that.setData({
                RID: res.data.RID,
                carID: res.data.carID,
                destination: res.data.destination,
                location: res.data.location,
                name: res.data.name,
                status: res.data.status,
                time: util.formatonlyMDTime(res.data.time),
                tel: res.data.tel,
                startTime: res.data.startTime
            })
            var passengers = res.data.passengers
            for (var i = 0; i <= passengers.length - 1; i++) {
                if (passengers[i].confirmTime) {
                    passengers[i].confirmTime = util.formatonlyHMTime(passengers[i].confirmTime)
                }
            }
            let roadslist = res.data.roadlist
            for (var i = 0; i <= roadslist.length - 1; i++) {
                if (roadslist[i].atime) {
                    roadslist[i].atime = util.formatonlyHMTime(roadslist[i].atime)
                }
                if (roadslist[i].ltime) {
                    roadslist[i].ltime = util.formatonlyHMTime(roadslist[i].ltime)
                }
            }
            let roadlist = util.formatRoadline(res.data);
            that.setData({
                passengers: passengers,
                roadlist: roadlist,
                roadslist:roadslist,
            })
        })
        wx.hideNavigationBarLoading();
        wx.stopPullDownRefresh();
    },
    onShow: function() {
        var that = this
        that.loadtravelInfo()
    },
    onPullDownRefresh: function() {
        var that = this
        that.loadtravelInfo()
    },
    call: function(e) {
        var that = this
        var tel = e.currentTarget.dataset.tel
        var name = e.currentTarget.dataset.name
        if (tel.length == 8) {
            tel = "00852" + tel
        }
        wx.showModal({
            title: '確認撥打',
            content: name + '-' + tel,
            success(res) {
                if (res.confirm) {
                    console.log('用户点击确定')
                    wx.makePhoneCall({
                        phoneNumber: tel
                    })
                } else if (res.cancel) {
                    console.log('用户点击取消')
                }
            }
        })
    },
    jumptomap() {
        wx.navigateTo({
            url: '../map/map?planrid=' + this.data.RID,
        })
    }
})