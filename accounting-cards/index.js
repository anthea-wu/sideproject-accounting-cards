const app = Vue.createApp({
    data(){
        return{
            cards:{
                list:{
                    data:{},
                    hidden: true,
                    add:{
                        name: '',
                        userId: '',
                    },
                    edit:{
                        name: '',
                        id: '',
                        createTime: '',
                    }
                },
                item:{
                    data:{},
                    card:{},
                    hidden: true,
                    oldCardId: '',
                    add:{
                        userId: '',
                        cardId: '',
                        name: '',
                        day: new Date().toJSON().substring(0,10),
                        time: '00:00',
                        count: 0,
                        createTime: ''
                    },
                    edit:{
                        userId: '',
                        cardId: '',
                        name: '',
                        day: new Date().toJSON().substring(0,10),
                        time: '00:00',
                        count: 0,
                        createTime: ''
                    }
                },
                is: {
                    adding: false,
                    deleting: false,
                    updating: false,
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
            },
        }
    },
    methods:{
        // about user
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
                    this.user.info.id = res.data.Id;
                    this.getCards();
                }).catch(err => {
                    console.log(err);
                    this.user.info.error = true;
                })
            }
        },
        // abount cards
        getCards: function () {
            axios({
                method: 'get',
                url: `./api/card/${this.user.info.id}/list`
            }).then(res => {
                this.cards.list.data = res.data;
                this.cards.list.hidden = false;
                this.user.session.get = true;
                this.user.session.news = false;
            }).catch(err => {
                apiFailed(err.response.status, err.response.data.Message);
            })
        },
        getCard: function (id) {
            axios({
                method: 'get',
                url: `./api/card/${this.user.info.id}/${id}`
            }).then(res => {
                this.cards.list.hidden = true;
                this.cards.item.hidden = false;
                this.cards.item.card = res.data;

                this.getDetails(this.cards.item.card.Id);
            }).catch(err => {
                apiFailed(err.response.status, err.response.data.Message);
            })
        },
        addCard: function () {
            if (this.cards.list.add.name === '' || this.cards.list.add.name.trim() === ''){
                Swal.fire({
                    icon: 'error',
                    text: '卡片名稱不能留白',
                    showConfirmButton: false,
                    timer: 2000,
                })
                return;
            }

            let duplicate = this.checkDuplicate(this.cards.list.add.name);

            if (duplicate){
                Swal.fire({
                    icon: 'error',
                    text: '卡片名稱已存在',
                    showConfirmButton: false,
                    timer: 2000,
                })

                this.cards.list.add.name = '';
                return;
            }

            this.cards.list.add.userId = this.user.info.id;
            this.cards.is.adding = true;
            axios({
                method: 'post',
                url: `./api/card`,
                data: this.cards.list.add
            }).then(res => {
                this.cards.list.data = res.data;
            }).catch(err => {
                apiFailed(err.response.status, err.response.data.Message);
            }).then(() => {
                this.cards.is.adding = false;
                this.cards.list.add.name = '';
            })
        },
        deleteCard: function (id) {
            let exist = this.cards.list.data.find(c => c.Id == id);
            if (exist.Name == "未分類"){
                swal.fire({
                    icon: 'warning',
                    text: '預設分類不能刪除',
                    showConfirmButton: false,
                    timer: 2000
                })
                return;
            }
            
            swal.fire({
                icon: 'warning',
                text: '卡片刪除後底下的明細也會自動刪除',
                showCancelButton: 'true',
                confirmButtonText: '刪除',
                cancelButtonText: '取消'
            }).then(res => {
                if (res.isConfirmed){
                    this.cards.is.deleting = true;
                    axios({
                        method: 'delete',
                        url: `./api/card`,
                        data: exist
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
                    }).then(() => {
                        this.cards.is.deleting = false;
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

            this.cards.list.edit.userId = this.user.info.id;
            this.cards.is.updating = true;
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
                this.getCards();
            }).catch(err => {
                apiFailed(err.response.status, err.response.data.Message);
            }).then(() => {
                this.cards.is.updating = false;
            })
        },
        setCardEditPage: function (card) {
            let vm = this;
            vm.cards.list.edit.id = card.Id;
            vm.cards.list.edit.name = card.Name;
            vm.cards.list.edit.createTime = card.CreateTime;
        },
        // about details
        getDetails: function (id) {
            axios({
                method: 'get',
                url: `./api/detail/${this.user.info.id}/${id}`
            }).then(res => {
                this.cards.item.details = res.data;
                this.cards.item.add.cardId = id;
            }).catch(err => {
                apiFailed(err.response.status, err.response.data.Message);
            })
        },
        addDetail: function () {
            if (this.cards.item.add.name === '' || this.cards.item.add.time.trim() === ''){
                Swal.fire({
                    icon: 'error',
                    text: '明細內容不得留空',
                    showConfirmButton: false,
                    timer: 2000,
                })
                return;
            }

            if (this.cards.item.add.count <= 0){
                Swal.fire({
                    icon: 'error',
                    text: '明細金額不得小於等於0',
                    showConfirmButton: false,
                    timer: 2000,
                })
                return;
            }

            this.cards.item.add.createTime = `${this.cards.item.add.day}T${this.cards.item.add.time}:00+00:00`;
            this.cards.item.add.userId = this.user.info.id;
            this.cards.item.add.cardId = this.cards.item.card.Id;
            
            axios({
                method: 'post',
                url: `./api/detail/item`,
                data: this.cards.item.add
            }).then(res => {
                this.cards.item.data = res.data;
            }).catch(err => {
                apiFailed(err.response.status, err.response.data.Message);
            }).then(() => {
                this.cards.item.add.name = '';
                this.cards.item.add.day = new Date().toJSON().substring(0,10);
                this.cards.item.add.time = '00:00';
                this.cards.item.add.count = 0;
                this.cards.item.add.date = '';

                this.getDetails(this.cards.item.card.Id);
            })
        },
        deleteDetail: function (exist) {
            axios({
                method: 'delete',
                url: `./api/detail/item`,
                data: exist
            }).then(res => {
                this.cards.item.data = res.data;
            }).catch(err => {
                apiFailed(err.response.status, err.response.data.Message);
            }).then(() => {
                this.getDetails(this.cards.item.card.Id);
            })
        },
        updateDetail: function () {
            this.cards.item.edit.createTime = `${this.cards.item.edit.day} ${this.cards.item.edit.time}`;
            this.cards.item.edit.userId = this.user.info.id;
            
            axios({
                method: 'put',
                url: `./api/detail/item?oldCardId=${this.cards.item.oldCardId}`,
                data: this.cards.item.edit
            }).then(res => {
                this.cards.item.data = res.data;
            }).catch(err => {
                apiFailed(err.response.status, err.response.data.Message);
            }).then(() => {
                this.getDetails(this.cards.item.card.Id);
            })
        },
        setDetailEditPage: function (item) {
            console.log(item);
            this.cards.item.edit.cardId = item.CardId;
            this.cards.item.oldCardId = item.CardId;
            this.cards.item.edit.id = item.Id;
            this.cards.item.edit.name = item.Name;
            this.cards.item.edit.count = item.Count;
            
            let formatDate = this.formatDate(item.CreateTime);
            this.cards.item.edit.day = formatDate.split(' ')[0];
            this.cards.item.edit.time = formatDate.split(' ')[1];
        },
        // others
        backToHome: function () {
            this.cards.list.hidden = false;
            this.cards.item.hidden = true;

            this.cards.item.add.name = '';
            this.cards.item.add.day = new Date().toJSON().substring(0,10);
            this.cards.item.add.time = '00:00';
            this.cards.item.add.count = 0;
            this.cards.item.add.createTime = '';
            
            this.getCards();
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
        if (localStorage.length !== 0){
            this.getSession();
        }
    }
});
app.mount('#app')