const app = Vue.createApp({
    data(){
        return{
            cards:{
                list:{
                    data:{},
                    hidden: true,
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
            },
            user: {
                session: {
                    get: false,
                    news: null,
                },
                info: {
                    id: '',
                    token: '',
                    error: false,
                },
                cards: {
                    list: {
                        hidden: false,
                        data: [],
                    }
                }
            }
        }
    },
    methods:{
        checkUser: function () {
            if (this.user.info.id === '') {
                this.postUser();
            } else {
                this.getSession();
            }
        },
        postUser: function () {
            axios({
                url: `./api/user/new`,
                method: 'post',
                data: this.user.info
            }).then(res => {
                this.user.info.id = res.data.Id;
                this.user.session.news = true;
                let info = JSON.stringify(res.data);
                localStorage.setItem('userInfo', info);
            }).catch(err => {
                console.log(err);
            })
        },
        getSession: function () {
            if (localStorage.length !== 0){
                let userInfoStr = localStorage.getItem('userInfo');
                let userInfo = JSON.parse(userInfoStr);
                this.user.info.id = userInfo.Id;
                this.getCards();
            }else{
                axios({
                    url: `./api/user/session/${this.user.info.id}`,
                    method: 'get'
                }).then(res => {
                    let info = JSON.stringify(res.data);
                    localStorage.setItem('userInfo', info);
                    this.getCards();
                }).catch(err => {
                    console.log(err);
                    this.user.info.error = true;
                })
            }
        },
        getCards: function () {
            let vm = this;

            axios({
                method: 'get',
                url: './api/card/list'
            }).then(res => {
                vm.cards.list.data = res.data;
                this.cards.list.hidden = false;
                this.user.session.get = true;
                this.user.session.news = false;
            }).catch(err => {
                apiFailed(err.response.status, err.response.data.Message);
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
                apiFailed(err.response.status, err.response.data.Message);
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

            let duplicate = this.checkDuplicate(vm.cards.list.add.name);

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
                apiFailed(err.response.status, err.response.data.Message);
            })
        },
        deleteCard: function (guid) {
            let exist = this.cards.list.data.find(c => c.Guid == guid);
            if (exist.Name == "未分類"){
                swal.fire({
                    icon: 'warning',
                    text: '預設分類不能刪除',
                    showConfirmButton: false,
                    timer: 2000
                })
                return;
            }
            
            let existDetails = [];
            
            swal.fire({
                icon: 'warning',
                text: '卡片刪除後底下的明細也會自動刪除',
                showCancelButton: 'true',
                confirmButtonText: '刪除',
                cancelButtonText: '取消'
            }).then(res => {
                if (res.isConfirmed){
                    axios({
                        method: 'get',
                        url: `./api/detail/list/${guid}`
                    }).then(res => {
                        existDetails = res.data.Details;
                        existDetails.forEach(item => {
                            axios({
                                method: 'delete',
                                url: `./api/detail/item/${item.Guid}`
                            }).then(res => {
                                console.log(`delete item: ${item.Name}`);
                            }).catch(err => {
                                apiFailed(err.response.status, `明細名稱 [${item.Name}] 刪除失敗`);
                                console.log(err)
                            })
                        });
                        axios({
                            method: 'delete',
                            url: `./api/card/${guid}`
                        }).then(res => {
                            this.cards.list.data = res.data;
                            swal.fire({
                                icon: 'success',
                                text: '刪除成功',
                                showConfirmButton: false,
                                timer: 1500
                            })
                        }).catch(err => {
                            apiFailed(err.response.status, err.response.data.Message);
                        })
                    }).catch(err => {
                        apiFailed(err.response.status, `卡片名稱 [${exist.Name}] 的明細取得失敗`);
                        console.log(err)
                    })
                }
            })            
        },
        updateCard: function () {
            let duplicate =  this.checkDuplicate(this.cards.list.edit.name);
            if (duplicate){
                Swal.fire({
                    icon: 'error',
                    text: '卡片名稱已存在',
                    showConfirmButton: false,
                    timer: 2000,
                });
                return;
            }
            
            axios({
                method: 'put',
                url: './api/card',
                data: this.cards.list.edit
            }).then(res => {
                swal.fire({
                    icon: 'success',
                    text: '卡片更新成功',
                    showConfirmButton: false,
                    timer: 2000,
                })
            }).catch(err => {
                apiFailed(err.response.status, err.response.data.Message);
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
                apiFailed(err.response.status, err.response.data.Message);
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
                apiFailed(err.response.status, err.response.data.Message);
            }).then(() => {
                vm.cards.item.add.name = '';
                vm.cards.item.add.day = new Date().toJSON().substring(0,10);
                vm.cards.item.add.time = '00:00';
                vm.cards.item.add.count = 0;
                vm.cards.item.add.date = '';
                
                this.getDetails(this.cards.item.card.Guid);
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
                apiFailed(err.response.status, err.response.data.Message);
            }).then(() => {
                this.getDetails(this.cards.item.card.Guid);
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
                apiFailed(err.response.status, err.response.data.Message);
            }).then(() => {
                this.getDetails(this.cards.item.card.Guid);
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
        checkDuplicate: function (name) {
            let duplicate = false;
            Array.from(this.cards.list.data).forEach((item, index, arr) => {
                if (item.Name == name) {
                    duplicate = true;
                    arr.splice(index, arr.length - index);
                    return;
                }
            })
            return duplicate;
        },
    },
    watch: {

    },
    mounted: function () {
        // this.getCards();
    }
});
app.mount('#app')