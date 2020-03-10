const config = require('../../config');
const util = require('../../utils/util.js');
const request = require('../../utils/request');
const app = getApp();
Page({
    data: {
        hint: "確認上車",
        RID: "",
        carID: "", //"粵S2W6N6",
        location: "", //"東莞火車站",
        locationATime: "",
        confirmTime: "",
        destination: "", //"三維正",
        driver: "", //"陳司機",
        time: "",
        status: 0,
        step: 0,
        time1: "(",
        time2: "5分鐘",
        time3: "上車)",
        result: true,
        forecast: "",
        roadlist: [],
        isIphoneX: false,
        showDriver: false,
        showCar: false,
        carImgUrls: [],
        driverImgUrls: [],
        driverID: "",
        timer:"",
    },
    onLoad: function() {
        var that = this
        if (app.globalData.carID) {
            that.checkCar()
        }
        that.setData({
            "isIphoneX": that.isIphoneX(),
        })
        timer: setInterval(function () {
            that.loadtravelInfo();
        }, 60000)
        // if (wx.getStorageSync("language")=="en"){
        //   wx.setNavigationBarTitle({
        //     title: "ShuttleBus"
        //   })
        // }
        console.log("type", app.globalData.list)
    },
    isIphoneX() { //根据iPhoneX适配
        let mobile = wx.getSystemInfoSync();
        if (mobile.model.indexOf("iPhone X") >= 0) {
            return true;
        } else {
            return false;
        }
    },
    // 下拉刷新
    onPullDownRefresh: function() {
        var that = this
        wx.showNavigationBarLoading();
        that.loadtravelInfo();
    },
    onShow: function() {
        var that = this
        that.loadtravelInfo();
        if (app.globalData.userType == "admin") {
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
                    "text": "行程列表" //that.data.language.tripList
                },
                {
                    "pagePath": "/pages/list/list",
                    "iconPath": "/image/icon_API.png",
                    "selectedIconPath": "/image/icon_API_HL.png",
                    "text": "任務管理"
                }
            ]
        }
        if (typeof this.getTabBar === 'function' &&
            this.getTabBar()) {
            this.getTabBar().setData({
                selected: 0, //这个数是，tabBar从左到右的下标，从0开始
                list: app.globalData.list
            })
        }
        console.log("type", app.globalData.userType)
    },
    // 读取行程信息
    loadtravelInfo: function() {
        var that = this
        let data = {
            sopenid: wx.getStorageSync('sopenid')
        }
        request.getTravel(data).then(res => {
            if (res.data.result == "null") {
                that.setData({
                    result: false
                })
            } else {
                that.setData({
                    RID: res.data.RID,
                    PlanRID: res.data.PlanRID,
                    carID: res.data.carID,
                    location: res.data.location, //util.translation(res.data.location, that.data.languages),
                    destination: res.data.destination, // util.translation(res.data.destination, that.data.languages),
                    driver: res.data.username,
                    time: res.data.time,
                    confirmTime: res.data.confirmTime,
                    status: res.data.status,
                    step: res.data.step,
                    tel: res.data.tel,
                    passengers: res.data.passengers,
                    type: res.data.type,
                    result: true,
                    roadlist: res.data.roadlist,
                    driverID: res.data.driverID,
                    // driverPhoto: res.data.driverPhoto,
                    // carPhoto: res.data.carPhoto,
                })
                // that.setDriver();
                // that.setCar();
                if (res.data.ForecastTime) {
                    that.setData({
                        arrivalTime: util.getDistanceTime(res.data.ForecastTime),
                    })
                }
                that.getforecastTime(res.data.location, res.data.destination, res.data.time)
                //获取车辆到达起點時間
                for (var i = 0; i < res.data.roadlist.length; i++) {
                    if (res.data.roadlist[i].location == that.data.location) {
                        that.setData({
                            locationATime: res.data.roadlist[i].atime,
                            locationLTime: res.data.roadlist[i].ltime,
                        })
                    } else if (res.data.roadlist[i].location == that.data.destination) {
                        that.setData({
                            destinationATime: res.data.roadlist[i].atime,
                        })
                    }
                }
                //預計到達出發點時間
                var time2 = util.getDistanceTime(res.data.time)
                if (time2 == "over") {
                    that.setData({
                        time1: "(預計",
                        time2: "5分鐘",
                        time3: "上車)",
                    })
                } else {
                    that.setData({
                        time1: "(",
                        time2: time2,
                        time3: "上車)",
                    })
                }
                //生成路線圖
                let roadlist = util.formatRoadline(res.data);
                that.setData({
                    roadlist: roadlist,
                })
            }
        })
        wx.hideNavigationBarLoading();
        wx.stopPullDownRefresh();
    },
    // 检查扫码车辆
    checkCar: function(e) {
        var that = this
        var carID = app.globalData.carID
        var that = this
        let data = {
            sopenid: wx.getStorageSync('sopenid'),
        }
        request.getTravel(data).then(res => {
            if (res.data.result == "null") {
                that.setData({
                    result: false
                })
            } else {
                that.setData({
                    carID: res.data.carID,
                    RID: res.data.RID,
                    confirmTime: res.data.confirmTime,
                })
                if (carID == that.data.carID) {
                    if (!res.data.confirmTime)
                        that.confirmByScan()
                } else {
                    wx.showModal({
                        title: '上錯車,請確認',
                        content: '您預約的車輛為' + that.data.carID,
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
            }
        })
    },
    myEventListener: function(e) {
        var that = this
        //获取到组件的返回值，并将其打印
        console.log('是否验证通过:' + e.detail.msg)
        if (e.detail.msg) {
            that.confirm()
        }
    },
    // 确认上车
    confirm: function() {
        var that = this
        let data = {
            sopenid: wx.getStorageSync('sopenid'),
            plsID: that.data.RID
        }
        request.passengerConfirm(data).then(res => {
            if (res.data.result == "success") {
                wx.showToast({
                    title: '上車成功',
                })
                that.loadtravelInfo()
            } else if (res.data.result == "fail") {
                wx.showToast({
                    title: '上車失败，请重试',
                })
                that.loadtravelInfo()
            }
        })
    },
    // 扫码确认上车
    confirmByScan: function() {
        var that = this
        let data = {
            sopenid: wx.getStorageSync('sopenid'),
            plsID: that.data.RID
        }
        request.passengerConfirm(data).then(res => {
            if (res.data.result == "success") {
                wx.showToast({
                    title: '上車成功',
                })
                that.loadtravelInfo()
                ////////////////////////////////////////////////乘客扫码
            } else if (res.data.result == "fail") {
                wx.showToast({
                    title: '上車失败，请重试',
                })
                that.loadtravelInfo()
            }
        })
    },
    station: function(e) {
        var no = e.currentTarget.dataset.no
        console.log(no)
    },
    openMap: function(e) {
        var id = e.currentTarget.dataset.id
        wx.navigateTo({
            url: '../map/map?id="' + id + '"',
        })
    },
    call: function(e) {
        var that = this
        var tel = e.currentTarget.dataset.tel
        if (tel.length == 8) {
            tel = "00852" + tel
        }
        wx.showModal({
            title: '確認撥打',
            content: that.data.driver + '-' + tel,
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
    up: function(e) {
        this.setData({
            down: null,
        })
    },
    down: function(e) {
        this.setData({
            down: "1",
        })
    },
    cancel: function(e) {
        var that = this
        wx.showModal({
            title: '提示',
            content: '確認要撤銷上車?',
            success(res) {
                if (res.confirm) {
                    console.log('用户点击确定')
                    let data = {
                        sopenid: wx.getStorageSync('sopenid'),
                        plsID: that.data.RID
                    }
                    request.passengerCancel(data).then(res => {
                        if (res.data.result == "success") {
                            wx.showToast({
                                title: '撤銷成功',
                            })
                            that.loadtravelInfo()
                        } else if (res.data.result == "fail") {
                            wx.showToast({
                                title: '撤銷失败，请重试',
                            })
                            that.loadtravelInfo()
                        }
                    })
                } else if (res.cancel) {
                    console.log('用户点击取消')
                }
            }
        })
    },
    getforecastTime: function(location, destination, time) {
        var that = this
        var latitude = ""
        var longitude = ""
        if (!that.data.confirmTime) {
            let data = {
                location: location,
                destination: destination,
                time: time,
            }
            request.getLineTime(data).then(res => {
                that.setData({
                    forecast: res.data.ForecastTime
                })
            })
        } else {
            wx.getLocation({
                success: function(res) {
                    console.log("[wx][getLocation]", res)
                    latitude = res.latitude
                    longitude = res.longitude
                    let data = {
                        latitude: latitude,
                        longitude: longitude,
                        destination: destination
                    }
                    request.getForecastTime(data).then(res => {
                        that.setData({
                            forecast: res.data.ForecastTime
                        })
                    })
                },
            })
        }
    },
    jumptomap() {
        wx.navigateTo({
            url: '../map/map?planrid=' + this.data.PlanRID,
        })
    },
    hide() {
        this.setData({
            showCar: false,
            showDriver: false
        })
    },
    showCar() {
        if (this.data.carImgUrls != "") {
            wx.previewImage({
                urls: this.data.carImgUrls
            })
        }
    },
    showDriver() {
        if (this.data.driverImgUrls != "") {
            wx.previewImage({
                urls: this.data.driverImgUrls
            })
        }
    },
    setCar() {
        let data = {
            sopenid: wx.getStorageSync('sopenid'),
            CarId: this.data.carID
        }
        let ps = [];
        for (var i = 0; i < this.data.carPhoto.length; i++) {
            var p = `${config.pbaseUrl}` + `Photo/Car/` + this.data.carPhoto[i].imgName + '.jpg';
            ps.push(p)
        }
        this.setData({
            carImgUrls: ps
        })
        // console.log(this.data.carImgUrls)
    },
    setDriver() {
        if (this.data.driverPhoto.length!=0){
            this.setData({
                driverImgUrls: [
                    `${config.pbaseUrl}` + `Photo/Driver/` + this.data.driverPhoto[0].imgName + '.jpg',
                ],
            })
        }
        // console.log(this.data.driverImgUrls)
    }
})