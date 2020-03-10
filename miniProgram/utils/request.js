const config = require('../config');
import Dialog from 'vant-weapp/dialog/dialog';
const service = {
    get(url, data) {
        return new Promise((resolve, reject) => {
            wx.request({
                method: 'get',
                url: url,
                data: data,
                header: {
                    "content-type": "application/json"
                },
                success: (res) => {
                    // 调用接口成功
                    this.requestLog(url, res);
                    this.errorMessage(res);
                    resolve(res);
                },
                fail: (err) => {
                    // 调用接口失败
                    this.errorMessage(err);
                    this.requestLog(url, err);
                    reject(err)
                }
            })
        })
    },
    post(url, data) {
        return new Promise((resolve, reject) => {
            wx.request({
                method: 'post',
                url: url,
                data: data,
                header: {
                    "content-type": "application/x-www-form-urlencoded"
                },
                success: (res) => {
                    // 调用接口成功
                    this.errorMessage(res);
                    this.requestLog(url, res);
                    resolve(res)
                },
                fail: (err) => {
                    // 调用接口失败
                    this.errorMessage(err);
                    this.requestLog(url, err);
                    reject(err)
                }
            })
        })
    },
    errorMessage(res) {
        if (res.statusCode != 200) {
            let code = res.statusCode.toString();
            if (code.substring(0, 1) == "4") {
                Dialog.alert({
                    title: '錯誤',
                    message: '請求出錯: ' + res.errMsg + '\n ErrorCode:' + res.statusCode,
                }).then(() => {});
            } else if (code.substring(0, 1) == "5") {
                Dialog.alert({
                    title: '錯誤',
                    message: '服務器異常: ' + res.errMsg + '\n ErrorCode:' + res.statusCode,
                }).then(() => {});
            } else {
                Dialog.alert({
                    title: '錯誤',
                    message: '異常: ' + res.errMsg + '\n ErrorCode:' + res.statusCode,
                }).then(() => {});
            }
        }
    },
    requestLog(url, res) {
        let str = url.split('/');
        let length = str.length - 1;
        let logHead = "[" + `${config.serverType}` + "][" + str[length] + "]:"
        if (res.data) {
            console.log(logHead, res.data);
        } else {
            console.log(logHead, res);
        }
    }
}
module.exports = {
    //登錄(獲取openID)
    login: (data) => {
        return new Promise((resolve, reject) => {
            resolve(service.post(`${config.baseUrl}/login`, data))
        })
    },
    //获取用户信息
    getUserInfo: (data) => {
        return new Promise((resolve, reject) => {
            resolve(service.post(`${config.baseUrl}/GetUserInfo`, data))
        })
    },
    //獲取行程
    getTravel: (data) => {
        return new Promise((resolve, reject) => {
            resolve(service.post(`${config.baseUrl}/GetTravel`, data))
        })
    },
    //獲取全程預計時間
    getLineTime: (data) => {
        return new Promise((resolve, reject) => {
            resolve(service.post(`${config.baseUrl}/GetLineTime`, data))
        })
    },
    //獲取預計到達時間
    getForecastTime: (data) => {
        return new Promise((resolve, reject) => {
            resolve(service.post(`${config.baseUrl}/GetForecastTime`, data))
        })
    },
    //用户绑定
    binding: (data) => {
        return new Promise((resolve, reject) => {
            resolve(service.post(`${config.baseUrl}/Binding`, data))
        })
    },
    //獲取任務詳情
    getMissionDetail: (data) => {
        return new Promise((resolve, reject) => {
            resolve(service.post(`${config.baseUrl}/GetMissionDetail`, data))
        })
    },

    getAllDriverList: (data) => {
        return new Promise((resolve, reject) => {
            resolve(service.post(`${config.baseUrl}/GetAllDriverList`, data))
        })
    },

    getAllCarList: (data) => {
        return new Promise((resolve, reject) => {
            resolve(service.post(`${config.baseUrl}/GetAllCarList`, data))
        })
    },

    getALLMissionList: (data) => {
        return new Promise((resolve, reject) => {
            resolve(service.post(`${config.baseUrl}/GetALLMissionList`, data))
        })
    },

    getMapInfo: (data) => {
        return new Promise((resolve, reject) => {
            resolve(service.post(`${config.baseUrl}/GetMapInfo`, data))
        })
    },

    getTravelList: (data) => {
        return new Promise((resolve, reject) => {
            resolve(service.post(`${config.baseUrl}/GetTravelList`, data))
        })
    },

    getMission: (data) => {
        return new Promise((resolve, reject) => {
            resolve(service.post(`${config.baseUrl}/GetMission`, data))
        })
    },

    driverUpdataGPSnew: (data) => {
        return new Promise((resolve, reject) => {
            resolve(service.post(`${config.baseUrl}/driverUpdataGPSnew`, data))
        })
    },

    driverConfirmA: (data) => {
        return new Promise((resolve, reject) => {
            resolve(service.post(`${config.baseUrl}/driverConfirmA`, data))
        })
    },

    dirverConfirm2: (data) => {
        return new Promise((resolve, reject) => {
            resolve(service.post(`${config.baseUrl}/dirverConfirm2`, data))
        })
    },
    dirverConfirm: (data) => {
        return new Promise((resolve, reject) => {
            resolve(service.post(`${config.baseUrl}/dirverConfirm`, data))
        })
    },

    driverCancel: (data) => {
        return new Promise((resolve, reject) => {
            resolve(service.post(`${config.baseUrl}/driverCancel`, data))
        })
    },

    driverSubmit: (data) => {
        return new Promise((resolve, reject) => {
            resolve(service.post(`${config.baseUrl}/driverSubmit`, data))
        })
    },

    getMissionList: (data) => {
        return new Promise((resolve, reject) => {
            resolve(service.post(`${config.baseUrl}/GetMissionList`, data))
        })
    },

    dirverMessage: (data) => {
        return new Promise((resolve, reject) => {
            resolve(service.post(`${config.baseUrl}/dirverMessage`, data))
        })
    },

    passengerConfirm: (data) => {
        return new Promise((resolve, reject) => {
            resolve(service.post(`${config.baseUrl}/passengerConfirm`, data))
        })
    },

    passengerCancel: (data) => {
        return new Promise((resolve, reject) => {
            resolve(service.post(`${config.baseUrl}/passengerCancel`, data))
        })
    },
}