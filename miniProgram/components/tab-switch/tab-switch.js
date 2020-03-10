Component({
  data: {
    tabTitle: [{}],
    curIndex: 0
  },
  lifetimes: {
    attached() {
      // 在组件实例进入页面节点树时执行
      this.tabChange({
        currentTarget: {
          dataset: {
            index: this.data.curIndex
          }
        }
      })
    }
  },
  properties: {
    tabTitle: {
      type: Array
    },
    tabName:{
      type:String
    },
    curIndex: {
      type: Number,
      observer(newVal){
        newVal > this.data.tabTitle.length - 1 ? console.log(`%c 下标“${newVal}”已超出默认范围(最大值可设“${this.data.tabTitle.length - 1}”)`,`color:#f00;`):console.log();
        newVal < 0 ? console.log(`%c 下标“${newVal}”不存在,最小可设置“0”`,`color:#f00;`):console.log();
      }
    }
  },
  methods: {
    tabChange(e) {
      let index = e.currentTarget.dataset.index;
      this.setData({
        idx:e.currentTarget.dataset.index,
        curIndex: index
      });
      this.triggerEvent('tabChange', { idx: e.currentTarget.dataset.index});
    }
  }
})