var app = getApp();
const config = require('../../config');
const request = require('../../utils/request.js');
Page({
    data: {
        markers: [],
        polyline: [],
        latitude: null,
        longitude: null,
        ready: null,
    },
    onLoad: function(options) {
        var that = this
        console.log(that.options.planrid)
        that.setData({
            height: app.globalData.windowHeight,
            planRID: that.options.planrid
        })
        that.loadInfo()
    },
    loadInfo: function() {
        var that = this
        that.setData({
            ready: false
        })
        let data = {
            sopenid: wx.getStorageSync('sopenid'),
            RID: that.data.planRID
        }
        request.getMapInfo(data).then(res => {
            that.setData({
                latitude: res.data.latitude,
                longitude: res.data.longitude,
                markers: [{
                        iconPath: "../../image/location.png",
                        id: 0,
                        latitude: res.data.Elatitude,
                        longitude: res.data.Elongitude,
                        width: 30,
                        height: 30,
                        callout: {
                            content: "" + res.data.name + "\n預計到達:" + res.data.ForecastTime,
                            display: "ALWAYS",
                            bgColor: "#48c23d",
                            borderColor: "#48c23d",
                            borderRadius: 10,
                            color: "#FFF",
                            padding: 5,
                            textAlign: "center",
                        }
                    },
                    {
                        iconPath: "../../image/Car_Top_Red.png",
                        id: 1,
                        latitude: res.data.latitude,
                        longitude: res.data.longitude,
                        width: 40,
                        height: 40,
                        callout: {
                            content: "" + res.data.carID + "\n更新時間" + res.data.gpsTime,
                            display: "ALWAYS",
                            bgColor: "#48c23d",
                            borderColor: "#48c23d",
                            borderRadius: 10,
                            color: "#FFF",
                            padding: 5,
                            textAlign: "center",
                        }
                    },
                ],
                polyline: [{
                    points: [{
                        longitude: res.data.longitude,
                        latitude: res.data.latitude,
                    }, {
                        longitude: res.data.Elongitude,
                        latitude: res.data.Elatitude,
                    }],
                    color: "#FF0000DD",
                    width: 2,
                    dottedLine: true
                }],
                ready: true
            })
            console.log(that.data.markers)
        })
    }
})