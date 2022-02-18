var app = angular.module('timeTrackingApp', []);

app.controller('projectsController', function ($scope, $http) {
    $scope.projects = $.parseJSON($('#projectsRecordsData').html());
    
    $scope.currentProject = {};

    $scope.editInfo = function (project) {
        $scope.currentProject = project;
        console.log(project);
        $('#projectModalEdit').show();
    }

    $scope.closeEditModal = function () {
        $('#projectModalEdit').hide();
    }
});

