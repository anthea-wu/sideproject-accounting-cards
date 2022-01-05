const app = Vue.createApp({
    data(){
        return{
            cards:{
                list:{
                    data:{},
                    hidden: false,
                    add:{
                        name: ''
                    },
                    edit:{
                        name: '',
                        guid: ''
                    }
                },
                item:{
                    data:{},
                    card:{},
                    hidden: true,
                    add:{
                        cardGuid: '',
                        name: '',
                        day: new Date().toJSON().substring(0,10),
                        time: '00:00',
                        count: 0,
                        date: ''
                    },
                    edit:{
                        cardGuid: '',
                        name: '',
                        day: new Date().toJSON().substring(0,10),
                        time: '00:00',
                        count: 0,
                        date: ''
                    }
                }
            }
        }
    },
    methods:{
        getCards: function () {
            let vm = this;

            axios({
                method: 'get',
                url: './api/card/list'
            }).then(res => {
                vm.cards.list.data = res.data;
            }).catch(err => {
                console.log(err);
            })
        },
        getCard: function (guid) {
            let vm = this;

            axios({
                method: 'get',
                url: `./api/card/${guid}`
            }).then(res => {
                vm.cards.list.hidden = true;
                vm.cards.item.hidden = false;
                vm.cards.item.card = res.data;

                vm.getDetails(vm.cards.item.card.Guid);
            }).catch(err => {
                console.log(err);
            })
        },
        addCard: function () {
            let vm = this;

            if (vm.cards.list.add.name === '' || vm.cards.list.add.name.trim() === ''){
                Swal.fire({
                    icon: 'error',
                    text: '卡片名稱不能留白',
                    showConfirmButton: false,
                    timer: 2000,
                })
                return;
            }

            let duplicate = false;
            Array.from(vm.cards.list.data).forEach((item, index, arr) => {
                if (item.Name == vm.cards.list.add.name) {
                    duplicate = true;
                    arr.splice(index, arr.length - index);
                    return;
                }
            })

            if (duplicate){
                Swal.fire({
                    icon: 'error',
                    text: '卡片名稱已存在',
                    showConfirmButton: false,
                    timer: 2000,
                })

                vm.cards.list.add.name = '';
                return;
            }

            axios({
                method: 'post',
                url: `./api/card`,
                data: vm.cards.list.add
            }).then(res => {
                vm.cards.list.data = res.data;
                vm.cards.list.add.name = '';
            }).catch(err => {
                console.log(err);
            })
        },
        deleteCard: function (guid) {
            let vm = this;

            axios({
                method: 'delete',
                url: `./api/card/${guid}`
            }).then(res => {
                vm.cards.list.data = res.data;
            }).catch(err => {
                console.log(err);
            })
        },
        updateCard: function () {
            let vm = this;
            console.log(vm.cards.list.edit);
            
            axios({
                method: 'put',
                url: './api/card',
                data: vm.cards.list.edit
            }).then(res => {
                console.log(res.data);
            }).catch(err => {
                console.log(err);
            })
        },
        setCardEditPage: function (card) {
            let vm = this;
            vm.cards.list.edit.guid = card.Guid;
            vm.cards.list.edit.name = card.Name;
        },
        getDetails: function (guid) {
            let vm = this;

            axios({
                method: 'get',
                url: `./api/detail/list/${guid}`
            }).then(res => {
                vm.cards.item.details = res.data.Details;
                vm.cards.item.add.cardGuid = res.data.CardGuid;
            }).catch(err => {
                console.log(err);
            })
        },
        addDetail: function () {
            let vm = this;

            if (vm.cards.item.add.name === '' || vm.cards.item.add.time.trim() === ''){
                Swal.fire({
                    icon: 'error',
                    text: '明細內容不得留空',
                    showConfirmButton: false,
                    timer: 2000,
                })
                return;
            }

            if (vm.cards.item.add.count <= 0){
                Swal.fire({
                    icon: 'error',
                    text: '明細金額不得小於等於0',
                    showConfirmButton: false,
                    timer: 2000,
                })
                return;
            }

            vm.cards.item.add.date = `${vm.cards.item.add.day} ${vm.cards.item.add.time}`;

            axios({
                method: 'post',
                url: `./api/detail/item`,
                data: vm.cards.item.add
            }).then(res => {
                vm.cards.item.data = res.data;
            }).catch(err => {
                console.log(err);
            }).then(() => {
                vm.cards.item.add.name = '';
                vm.cards.item.add.day = new Date().toJSON().substring(0,10);
                vm.cards.item.add.time = '00:00';
                vm.cards.item.add.count = 0;
                vm.cards.item.add.date = '';
            })
        },
        deleteDetail: function (guid) {
            let vm = this;

            axios({
                method: 'delete',
                url: `./api/detail/item/${guid}`
            }).then(res => {
                vm.cards.item.data = res.data;
            }).catch(err => {
                console.log(err);
            })
        },
        updateDetail: function () {
            let vm = this;

            vm.cards.item.edit.date = `${vm.cards.item.edit.day} ${vm.cards.item.edit.time}`;
            
            axios({
                method: 'put',
                url: `./api/detail/item`,
                data: vm.cards.item.edit
            }).then(res => {
                vm.cards.item.data = res.data;
            }).catch(err => {
                console.log(err);
            })
        },
        setDetailEditPage: function (item) {
            let vm = this;
            
            vm.cards.item.edit.cardGuid = item.CardGuid;
            vm.cards.item.edit.guid = item.Guid;
            vm.cards.item.edit.name = item.Name;
            vm.cards.item.edit.count = item.Count;
            
            let formatDate = vm.formatDate(item.Date);
            vm.cards.item.edit.day = formatDate.split(' ')[0];
            vm.cards.item.edit.time = formatDate.split(' ')[1];
            
            vm.cards.item.edit.cardGuid = item.CardGuid;
        },
        backToHome: function () {
            let vm = this;

            vm.cards.list.hidden = false;
            vm.cards.item.hidden = true;

            vm.cards.item.add.name = '';
            vm.cards.item.add.day = new Date().toJSON().substring(0,10);
            vm.cards.item.add.time = '00:00';
            vm.cards.item.add.count = 0;
            vm.cards.item.add.date = '';
        },
        formatDate: function (date) {
            return `${date.split('T')[0]} ${date.split('T')[1].split('+')[0]}`
        },
    },
    watch: {

    },
    mounted: function () {
        this.getCards();
    }
});
app.mount('#app')