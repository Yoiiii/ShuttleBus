Component({
  properties: {
    travelList: {
      type: Object
    }
  },
  lifetimes: {
    //组件的生命周期函数
    attached() {
    //   console.log("properties",this.data.travelList)
    },
  },
  methods: {
    showDetail: function(e) {
      var planRID = this.data.travelList.PlanRID
      // wx.navigateTo({
      //   url: '../detail/detail?planRID=' + planRID,
      // });
      console.log("list-item.js", planRID)
      var myEventDetail = {
         'planRID': planRID
         }
      this.triggerEvent('showDetail', myEventDetail ,"6666") 
    },
  }
})