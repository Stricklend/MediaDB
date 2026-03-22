
document.addEventListener('DOMContentLoaded', function () {
    // 이미지 미리보기
    document.getElementById('AlbumImage').addEventListener('change', function (e) {
        const file = e.target.files[0];
        const preview = document.getElementById('preview');
        if (file) {
            const reader = new FileReader();
            reader.onload = function (ev) {
                preview.src = ev.target.result;
                preview.style.display = '';
            };
            reader.readAsDataURL(file);
        } else {
            preview.src = '/Content/photo-icon.png'; // 디폴트 이미지 경로로 변경
            preview.style.display = '';
        }
    });

    // 초기화 버튼
    document.getElementById('ResetButton').addEventListener('click', function () {
        document.getElementById('Artist').value = '';
        document.getElementById('AlbumName').value = '';
        document.getElementById('AlbumImage').value = '';
        document.getElementById('preview').src = '/Content/photo-icon.png'; // 디폴트 이미지 경로로 변경
        document.getElementById('preview').style.display = '';
        document.getElementById('Message').textContent = '';
        document.getElementById('RegisterButton').disabled = false;
    });
});

