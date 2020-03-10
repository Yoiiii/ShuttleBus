Page({
    data: {
        imgUrls: [
            'http://desk-fd.zol-img.com.cn/g5/M00/02/05/ChMkJ1bKyZmIWCwZABEwe5zfvyMAALIQABa1z4AETCT730.jpg',
            'http://desk-fd.zol-img.com.cn/g5/M00/02/05/ChMkJ1bKyZmIWCwZABEwe5zfvyMAALIQABa1z4AETCT730.jpg',
            'http://desk-fd.zol-img.com.cn/g5/M00/02/05/ChMkJ1bKyZmIWCwZABEwe5zfvyMAALIQABa1z4AETCT730.jpg',
            'https://web1.seeds-garment.com.cn/STBusSystem/images/404.png'
        ],
        show: true
    },
    hide() {
        console.log('component hide')
        setTimeout(() => {
            console.log('component show')
            this.setData({
                show: true
            })
        }, 1000)
    }
});