window.startCounter = () => {
    function animateValue(id, start, end, duration) {
        let range = end - start;
        let current = start;
        let increment = end > start ? 1 : -1;
        let stepTime = Math.abs(Math.floor(duration / range));
        const obj = document.getElementById(id);
        const timer = setInterval(function () {
            current += increment;
            obj.innerText = current.toLocaleString();
            if (current == end) {
                clearInterval(timer);
            }
        }, stepTime);
    }

    animateValue("counter1", 0, 123, 1000);   // Đối tác
    animateValue("counter2", 0, 135, 1200);   // Đơn hàng
    animateValue("counter3", 0, 99, 800);     // Tin cậy
    animateValue("counter4", 0, 24, 800);     // Hỗ trợ
};
