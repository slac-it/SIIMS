
$(document).ready(function ($) {

    if (window.history && window.history.pushState) {

        $(window).on('popstate', function () {
            var hashLocation = location.hash;
            var hashSplit = hashLocation.split("#!/");
            var hashName = hashSplit[1];

            if (hashName !== '') {
                var hash = window.location.hash;
                if (hash === '') {
                    if (confirm("Heads Up: SAVE your work before leaving this page. Choose Cancel to stay here and Save. Sure you are ready to leave this page?")) {
                        window.location = 'default.aspx';
                        return false;
                    }
                }
            }
        });

        window.history.pushState('forward', null, './#forward');
    }

});

