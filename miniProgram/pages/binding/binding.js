    // miniprogram/pages/binding/binding.js
  const config = require('../../config');
  const util = require('../../utils/util.js');
  const request = require('../../utils/request.js');
  import Notify from 'vant-weapp/notify/notify';
  const app = getApp() 
  Page({
      data: {
          phone: '',
          number: '',
          language: ''
      },
      onLoad: function(options) {
          var that = this
        //that.setLanguage()
      },
      setLanguage() {
          this.setData({
              language: wx.getStorageSync("languages")
          });
      },
      // 获取输入账号 
      phoneInput: function(e) {
          this.setData({
              phone: e.detail.value
          })
      },
      // 获取输入密码 
      numberInput: function(e) {
          this.setData({
              number: e.detail.value
          })
      },
      binding: function() {
          var that = this
          if (that.data.phone.length == 0 || that.data.number.length == 0) {
              Notify('手機號和工號不能為空');
          } else if (that.data.phone.length != 11 && that.data.phone.length != 8) {
              Notify('請輸入正確手機號');
          } else {
              wx.login({
                  success(res) {
                      if (res.code) {
                          let data = {
                              code: res.code,
                              tel: that.data.phone,
                              userID: that.data.number
                          }
                          request.binding(data).then(res => {
                              if (res.data.result == "false") {
                                  Notify('手機號或工號輸入錯誤');
                              } else if (res.data.result == "true") {
                                  wx.showModal({
                                      title: '綁定成功',
                                      content: '點擊確認跳轉',
                                      success(res) {
                                          if (res.confirm) {
                                              wx.navigateTo({
                                                  url: '../start/start',
                                              })
                                          }
                                      }
                                  })
                              } else if (res.data.result == 0) {
                                  Notify('該手機號或工號已綁定');
                              }
                          })
                      }
                  }
              })
          }
      }
  })