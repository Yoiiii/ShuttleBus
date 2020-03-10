// pages/test/test.js
Page({

  /**
   * 页面的初始数据
   */
  data: {

  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {

  },

  /**
   * 生命周期函数--监听页面初次渲染完成
   */
  onReady: function () {

  },

  /**
   * 生命周期函数--监听页面显示
   */
  onShow: function () {

  },

  /**
   * 生命周期函数--监听页面隐藏
   */
  onHide: function () {

  },

  /**
   * 生命周期函数--监听页面卸载
   */
  onUnload: function () {

  },

  /**
   * 页面相关事件处理函数--监听用户下拉动作
   */
  onPullDownRefresh: function () {

  },

  /**
   * 页面上拉触底事件的处理函数
   */
  onReachBottom: function () {

  },

  /**
   * 用户点击右上角分享
   */
  onShareAppMessage: function () {

  },
  sendmessage() {
    let that = this
    console.log("requestSubscribeMessage");
    wx.requestSubscribeMessage({
      tmplIds: [
        'E0DxOf32PbKGx02AEinTFtGsvaSCQx_7_j_bsGjdHNQ',
      ],
      success(res) {
        console.log(res)
        for (var key in res) {
          if (key != 'errMsg') {
            if (res[key] == 'reject') {
              wx.showModal({
                title: '订阅消息',
                content: '您已拒绝了订阅消息，如需重新订阅请前往设置打开。',
                confirmText: '去设置',
                //showCancel: false,
                success: res => {
                  if (res.confirm) {
                    wx.openSetting({})
                  }
                }
              })
              return
            } else {
              wx.showToast({
                title: '订阅成功'
              })
              that.senddingyue();
            }
          }
        }
      },
      fail(res) {
        console.log(res) /*20004-errorCode*/
        wx.showModal({
          title: '订阅消息',
          content: '您关闭了“接收订阅信息”，请前往设置打开！',
          confirmText: '去设置',
          showCancel: false,
          success: res => {
            if (res.confirm) {
              wx.openSetting({})
            }
          }
        })
      },
    })
  },
})