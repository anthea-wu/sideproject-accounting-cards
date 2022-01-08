function apiFailed(status, body) {
    Swal.fire({
        icon: 'error',
        title: status,
        text: body,
        showConfirmButton: false,
        timer: 2000,
    })
}