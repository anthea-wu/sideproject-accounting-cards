function apiFailed(status, body) {
    Swal.fire({
        icon: 'error',
        title: status,
        text: body,
        showConfirmButton: false,
        timer: 2000,
    })
}

function apiSuccess(text) {
    Swal.fire({
        icon: 'success',
        title: text,
        showConfirmButton: false,
        timer: 2000,
    })
}