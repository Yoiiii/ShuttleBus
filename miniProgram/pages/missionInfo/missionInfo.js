const config = require('../../config');
var util = require('../../utils/util.js');
const request = require('../../utils/request');
import Notify from 'vant-weapp/notify/notify';
import Dialog from 'vant-weapp/dialog/dialog';
Page({
    data: {
        hint: "",
        result: true,
        roadlist: [],
        latitude: null,
        longitude: null,
        speed: 0,
        timer: "",
        pause: false,
        background: "front",
        module: "",
        timer2: "",
    },
    onLoad: function(options) {
        var that = this
        that.setData({
            "isIphoneX": that.isIphoneX(),
            timer2: setInterval(function() {
                that.loadtravelInfo();
            }, 60000)
        })
    },
    isIphoneX() {
        let mobile = wx.getSystemInfoSync();
        if (mobile.model.indexOf("iPhone X") >= 0) {
            return true;
        } else {
            return false;
        }
    },
    onReady: function() {
        var that = this
    },
    onShow: function() {
        var that = this
        if (typeof this.getTabBar === 'function' &&
            this.getTabBar()) {
            this.getTabBar().setData({
                selected: 0, //这个数是，tabBar从左到右的下标，从0开始
            })
        }
        that.setData({
            background: "front",
        })
        that.loadtravelInfo()
    },
    onHide: function() {
        var that = this
        that.setData({
            background: "back",
        })
        console.log("onHide", that.data.background)
    },
    onPullDownRefresh: function() {
        var that = this
        wx.showNavigationBarLoading();
        that.loadtravelInfo()
    },
    loadtravelInfo: function(e) {
        var that = this
        let data = {
            sopenid: wx.getStorageSync('sopenid'),
        }
        request.getMission(data).then(res => {
            if (res.data.result != "null") {
                var time = util.getDistanceTime(res.data.time)
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
                    time: time,
                    hint: res.data.name,
                    result: true,
                    planID: res.data.planID,
                    module: res.data.module,
                })
                that.sliders = that.selectComponent("#sliders");
                that.sliders.recovery()
                //執行後台自動更新gps信息方法
                if (that.data.status == 1 && that.data.pause == false && that.data.module == "") {
                    that.updateRealtimeGps()
                } else {
                    clearInterval(that.data.timer)
                    that.setData({
                        timer: null
                    })
                    wx.stopLocationUpdate({
                        success(res) {
                            console.log('关闭监听实时位置变化', res)
                        },
                        fail(res) {
                            console.log('关闭监听实时位置变化', res)
                        }
                    })
                }
                //滑塊顯示
                for (var q = 0; 1 < res.data.roadlist.length; q++) {
                    if (res.data.roadlist[q].atime == "" && res.data.roadlist[q].ltime == "" && q == 0) //未到出發點
                    {
                        if (res.data.status == 1) {
                            that.setData({
                                nowlocation: res.data.roadlist[q].location,
                                confirmType: "A",
                                hint: "到達" + res.data.roadlist[q].location
                            })
                        } else {
                            that.setData({
                                nowlocation: res.data.roadlist[q].location,
                                confirmType: "L",
                                hint: "發車"
                            })
                        }
                        break;
                    } else if (res.data.roadlist[q].atime != "" && res.data.roadlist[q].ltime == "") //到達未出發
                    {
                        if (res.data.status == 2) {
                            that.setData({
                                nowlocation: res.data.roadlist[q].location,
                                confirmType: "F",
                                hint: "已到達終點，確認完成"
                            })
                        } else {
                            that.setData({
                                nowlocation: res.data.roadlist[q].location,
                                confirmType: "L",
                                hint: "從" + res.data.roadlist[q].location + "出發"
                            })
                        }
                        break;
                    } else {
                        if (q != 0) {
                            if (res.data.roadlist[q].atime == "" && res.data.roadlist[q].ltime == "" && res.data.roadlist[q - 1].ltime != "") {
                                that.setData({
                                    nowlocation: res.data.roadlist[q].location,
                                    confirmType: "A",
                                    hint: "到達" + res.data.roadlist[q].location
                                })
                                break;
                            }
                        }
                    }
                }
                //that.autoLeave() //執行自動確認出發
                //路線圖
                var roadlist = []
                var x = 1
                var roads = {}
                var car = ""
                var road = ""
                var location = ""
                for (var i = 0; i <= res.data.roadlist.length - 1; i++) {
                    if (i == 0) {
                        location = res.data.roadlist[i].location
                        if (res.data.roadlist[0].atime) {
                            road = "roads"
                            if (res.data.roadlist[i].ltime == "") {
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
                            if (res.data.status == "0") {
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
                    } else if (i == res.data.roadlist.length - 1) {
                        location = res.data.roadlist[i].location
                        road = "roade"
                        if (res.data.roadlist[i].ltime == "" && res.data.roadlist[i].atime != "") {
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
                        location = res.data.roadlist[i].location
                        if (res.data.roadlist[i].ltime == "" && res.data.roadlist[i].atime != "") {
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
                    if (i != res.data.roadlist.length - 1) {
                        if (res.data.roadlist[i].ltime != "" && res.data.roadlist[i + 1].atime == "") {
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
                that.setData({
                    roadlist: roadlist,
                })
            } else {
                that.setData({
                    result: false,
                })
            }
        })
        wx.hideNavigationBarLoading();
        wx.stopPullDownRefresh();
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
    updateRealtimeGps: function() {
        var that = this
        wx.startLocationUpdateBackground({
            success(res) {
                console.log('开启后台定位', res)
            },
            fail(res) {
                console.log('开启后台定位失败', res)
            }
        })
        wx.getLocation({
            type: "gcj02",
            success: function(res) {
                console.log("[wx][getLocation1]", res)
                that.setData({
                    latitude: res.latitude,
                    longitude: res.longitude,
                })
                let data = {
                    sopenid: wx.getStorageSync('sopenid'),
                    rid: that.data.RID,
                    planID: that.data.planID,
                    longitude: that.data.longitude,
                    latitude: that.data.latitude,
                    speed: that.data.speed,
                    status: that.data.background,
                }
                request.driverUpdataGPSnew(data).then(res => {
                    if (res.data.result == "Leave") {
                        clearInterval(that.data.timer)
                        wx.showModal({
                            title: '定位離開',
                            content: '已離開' + that.data.nowlocation,
                            showCancel: false,
                            confirmText: "確定",
                            success(res) {
                                if (res.confirm) {
                                    console.log('用户点击确定')
                                    that.loadtravelInfo()
                                } else if (res.cancel) {
                                    console.log('用户点击取消')
                                    that.loadtravelInfo()
                                }
                            }
                        })
                        /*Dialog.alert({
                          title: '标题',
                          message: '離開' + that.data.nowlocation
                        }).then(() => {
                        });*/
                        that.loadtravelInfo();
                    } else if (res.data.result == "Arrive") {
                        clearInterval(that.data.timer)
                        wx.showModal({
                            title: '定位到達',
                            content: '已到達' + that.data.nowlocation,
                            showCancel: false,
                            confirmText: "確定",
                            success(res) {
                                if (res.confirm) {
                                    console.log('用户点击确定')
                                    that.loadtravelInfo()
                                } else if (res.cancel) {
                                    console.log('用户点击取消')
                                    that.loadtravelInfo()
                                }
                            }
                        })
                        /*Dialog.alert({
                          title: '定位',
                          message: '到達' + that.data.nowlocation
                        }).then(() => {
                        });*/
                        that.loadtravelInfo();
                    } else if (res.data.result == "success") {
                        Notify({
                            text: '定位信息上傳成功',
                            duration: 1000,
                            selector: '#blue-notify',
                            backgroundColor: '#1989fa' //#07c160
                        });
                    }
                })
            },
        })
        /*wx.onLocationChange(function (res) {
          console.log('[wx][onLocationChange]', res)
          that.setData({
            longitude: res.longitude,
            latitude: res.latitude,
            speed: res.speed,
          })
        })*/
        that.setData({
            timer: setInterval(function() {
                //循环执行代码
                wx.getLocation({
                    type: "gcj02",
                    success: function(res) {
                        console.log("[wx][getLocation2]", res)
                        that.setData({
                            latitude: res.latitude,
                            longitude: res.longitude,
                        })
                        let data = {
                            sopenid: wx.getStorageSync('sopenid'),
                            rid: that.data.RID,
                            longitude: that.data.longitude,
                            latitude: that.data.latitude,
                            speed: that.data.speed,
                            planID: that.data.planID,
                            status: that.data.background,
                        }
                        request.driverUpdataGPSnew(data).then(res => {
                            if (res.data.result == "Leave") {
                                clearInterval(that.data.timer)
                                wx.showModal({
                                    title: '定位離開',
                                    content: '已離開' + that.data.nowlocation,
                                    showCancel: false,
                                    confirmText: "確定",
                                    success(res) {
                                        if (res.confirm) {
                                            console.log('用户点击确定')
                                            that.loadtravelInfo()
                                        } else if (res.cancel) {
                                            console.log('用户点击取消')
                                            that.loadtravelInfo()
                                        }
                                    }
                                })
                                /*Dialog.alert({
                                  title: '标题',
                                  message: '離開'+that.data.nowlocation
                                }).then(() => {
                                });*/
                                that.loadtravelInfo();
                            } else if (res.data.result == "Arrive") {
                                clearInterval(that.data.timer)
                                wx.showModal({
                                    title: '定位到達',
                                    content: '已到達' + that.data.nowlocation,
                                    showCancel: false,
                                    confirmText: "確定",
                                    success(res) {
                                        if (res.confirm) {
                                            console.log('用户点击确定')
                                            that.loadtravelInfo()
                                        } else if (res.cancel) {
                                            console.log('用户点击取消')
                                            that.loadtravelInfo()
                                        }
                                    }
                                })
                                /*Dialog.alert({
                                  title: '定位',
                                  message: '到達'+that.data.nowlocation
                                }).then(() => {
                                });*/
                                that.loadtravelInfo();
                            } else if (res.data.result == "success") {
                                Notify({
                                    text: '定位信息上傳成功',
                                    duration: 1000,
                                    selector: '#blue-notify',
                                    backgroundColor: '#1989fa' //#07c160
                                });
                            }
                        })
                    },
                })
            }, 30000)
        })
    },
    /*driverUpdataGPS: function () {
      var that = this
      wx.getLocation({
        type: 'gcj02',
        success: function (res) {
          console.log("[wx][getLocation]gcj02:", res)
          that.setData({
            longitude: res.longitude,
            latitude: res.latitude,
          })
          wx.request({
            url: `${config.baseUrl}/WeChatWS.asmx/driverUpdataGPS`,
            data: {
              sopenid: wx.getStorageSync('sopenid'),
              rid: that.data.RID,
              longitude: res.longitude,
              latitude: res.latitude,
            },
            method: "post",
            success(res) {
              console.log("[webserver][driverUpdataGPS]:", res.data)
              if (res.data.result == "success") {
                Notify({
                  text: '定位信息上傳成功',
                  duration: 1000,
                  selector: '#blue-selector',
                  backgroundColor: '#1989fa'
                });
                /*wx.showToast({
                  icon: "none",
                  title: '定位信息上傳成功',
                })
                if (res.data.arrival) {
                  wx.showModal({
                    title: '定位到達',
                    content: '已到達' + res.data.arrival,
                    showCancel: false,
                    confirmText: "確定",
                    success(res) {
                      if (res.confirm) {
                        console.log('用户点击确定')
                        that.loadtravelInfo()
                      } else if (res.cancel) {
                        console.log('用户点击取消')
                      }
                    }
                  })
                }*/
    /*} else if (res.data.result == "fail") {
                wx.showToast({
                  icon: "none",
                  title: '定位信息上傳失敗',
                })
              }
            }
          })
        },
        fail(res) {
          wx.showModal({
            title: '獲取定位失敗',
            content: '請確認是否打開GPS，和給小程序權限',
            showCancel: false,
            confirmText: "關閉",
            success(res) {
              if (res.confirm) {
                console.log('用户点击确定')
              } else if (res.cancel) {
                console.log('用户点击取消')
              }
            }
          })
        }
      })
    },*/
    myEventListener: function(e) {
        var that = this
        //获取到组件的返回值，并将其打印
        if (e.detail.msg) {
            if (that.data.confirmType == "F") {
                wx.showModal({
                    title: '確認完成',
                    content: '確認完成后無法再進行撤回',
                    success(res) {
                        if (res.confirm) {
                            console.log('用户点击确定')
                            let data = {
                                sopenid: wx.getStorageSync('sopenid'),
                                rid: that.data.RID,
                                confirmType: that.data.confirmType,
                            }
                            request.driverSubmit(data).then(res => {
                                if (res.data.result == "success") {
                                    wx.showToast({
                                        title: '提交成功',
                                    })
                                    that.loadtravelInfo()
                                } else if (res.data.result == "fail") {
                                    wx.showToast({
                                        title: '提交失败，请重试',
                                    })
                                    that.loadtravelInfo()
                                }
                            })
                        } else if (res.cancel) {
                            console.log('用户点击取消')
                        }
                    }
                })
            } else {
                let data = {
                    sopenid: wx.getStorageSync('sopenid'),
                    rid: that.data.RID,
                    confirmType: that.data.confirmType,
                }
                request.driverConfirmA(data).then(res => {
                    if (res.data.result == "success") {
                        wx.showToast({
                            title: '提交成功',
                        })
                        that.setData({
                            pause: false
                        })
                        that.loadtravelInfo()
                    } else if (res.data.result == "fail") {
                        wx.showToast({
                            title: '提交失败，请重试',
                        })
                        that.loadtravelInfo()
                    }
                })
            }
        }
    },
    driverCheck: function(e) {
        var that = this
        console.log('switch1 发生 change 事件，携带值为', e.detail.value)
        var RID = e.currentTarget.dataset.rid
        var confirmTime = e.currentTarget.dataset.confirmTime
        if (e.detail.value) {
            let data = {
                RID: RID,
                sopenid: wx.getStorageSync('sopenid'),
            }
            request.dirverConfirm(data).then(res => {
                if (res.data.result == "success") {
                    wx.showToast({
                        title: '提交成功',
                    })
                    that.loadtravelInfo()
                } else if (res.data.result == "fail") {
                    wx.showToast({
                        title: '提交失败，请重试',
                    })
                    that.loadtravelInfo()
                }
            })
        } else {
            let data = {
                RID: RID,
                sopenid: wx.getStorageSync('sopenid'),
            }
            request.dirverConfirm2(data).then(res => {
                if (res.data.result == "success") {
                    wx.showToast({
                        title: '提交成功',
                    })
                    that.loadtravelInfo()
                } else if (res.data.result == "fail") {
                    wx.showToast({
                        title: '提交失败，请重试',
                    })
                    that.loadtravelInfo()
                }
            })
        }
    },
    cancel: function(e) {
        var that = this
        wx.showModal({
            title: '提示',
            content: '確認要撤銷返回上一步?',
            success(res) {
                if (res.confirm) {
                    console.log('用户点击确定')
                    let data = {
                        sopenid: wx.getStorageSync('sopenid'),
                        rid: that.data.RID,
                        cancelType: that.data.confirmType,
                    }
                    request.driverCancel(data).then(res => {
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
    /*autoLeave: function (e) {
      var that = this;
      var latitude = "";
      var longitude = "";
      var hit = 0;
      if (that.data.confirmType == "L" && that.data.hint != "發車") {
        wx.startLocationUpdateBackground({
          success(res) {
            console.log('开启监听实时位置变化', res)
          },
          fail(res) {
            console.log('开启监听实时位置变化失败', res)
          }
        })
        wx.request({
          url: `${config.baseUrl}/WeChatWS.asmx/GetLocation`,
          data: {
            location: that.data.nowlocation
          },
          method: "post",
          success(res) {
            console.log('[webserver][GetLocation]', [res.data[0].latitude, res.data[0].longitude])
            latitude = res.data[0].latitude;
            longitude = res.data[0].longitude;
            wx.onLocationChange(function (res) {
              console.log('[wx][onLocationChange]', res)
              var distance = util.getGreatCircleDistance(latitude, longitude, res.latitude, res.longitude)
              console.log("distance", [latitude, longitude, res.latitude, res.longitude])
              console.log('距離和速度', [distance, res.speed])
              if (distance >= 200 || res.speed >= 3) {
                if (hit == 0) {
                  hit = hit + 1
                  setTimeout(function () {
                    wx.request({
                      url: `${config.baseUrl}/WeChatWS.asmx/driverConfirmA`,
                      data: {
                        sopenid: wx.getStorageSync('sopenid'),
                        rid: that.data.RID,
                        confirmType: that.data.confirmType,
                      },
                      method: "post",
                      success(res) {
                        console.log("[webserver][dirverConfirmA]:", res.data)
                        if (res.data.result == "success") {
                          hit == 1
                          wx.showModal({
                            title: '定位離開',
                            content: '已離開' + that.data.nowlocation,
                            showCancel: false,
                            confirmText: "確定",
                            success(res) {
                              if (res.confirm) {
                                console.log('用户点击确定')
                                that.loadtravelInfo()
                              } else if (res.cancel) {
                                console.log('用户点击取消')
                                that.loadtravelInfo()
                              }
                            }
                          })
                          wx.stopLocationUpdate({
                            success(res) {
                              console.log('关闭监听实时位置变化', res)
                            },
                            fail(res) {
                              console.log('关闭监听实时位置变化', res)
                            }
                          })
                        } else {
                          hit = 0
                        }
                      }
                    })
                    //要延时执行的代码
                  }, 1000)
                }
              }
            })
          }
        })
      } else if (that.data.confirmType == "A") {
        wx.stopLocationUpdate({
          success(res) {
            console.log('关闭监听实时位置变化', res)
          },
          fail(res) {
            console.log('关闭监听实时位置变化', res)
          }
        })
      }
    },*/
    pause: function() {
        var that = this
        clearInterval(that.data.timer)
        that.setData({
            timer: null,
            pause: true
        })
        wx.stopLocationUpdate({
            success(res) {
                console.log('关闭监听实时位置变化', res)
            },
            fail(res) {
                console.log('关闭监听实时位置变化', res)
            }
        })
    },
    goon: function() {
        var that = this
        that.setData({
            pause: false
        })
        that.loadtravelInfo()
    },
})