Component({
    properties: {
        roadlist: {
            type: Array
        },
        planrid: {
            type: String
        },
        type: {
            type: String
        }
    },
    lifetimes: {},
    methods: {
        jumptomap: function(e) {
            this.triggerEvent('jumptomap')
        },
        bindtapCar: function(e) {
            this.triggerEvent('showCar')
        }
    }
})