// pipeline{
//     agent{
//         label "agent"
//     }
//     environment {
//         APP_NAME = "reverse proxy"
//         RELEASE = "1.0.0"
//         DOCKER_USER = "hiastdevops"
//         DOCKER_PASS = 'dockerhub'
//         KUBE_CREDENTAILS = '/home/ubuntu/personal/jenkins-demo/config'
//         IMAGE_NAME = "${DOCKER_USER}" + "/" + "${APP_NAME}"
//         IMAGE_TAG = "${RELEASE}-${BUILD_NUMBER}"
//     }
//     stages{
//         stage("Cleanup Workspace"){
//             steps {
//                 cleanWs()
//             }

//         }
    
//         stage("Git SCM"){
//             steps {
//                 git branch: 'main', credentialsId: 'github', url: 'https://github.com/MohamadAlturky/Membership'
//                 sh "ls"
//             }

//         }

//         // stage("Build & Push Docker Image") {
//         //     steps {
//         //         script {
//         //             docker.withRegistry('',DOCKER_PASS) {
//         //                 docker_image = docker.build "${IMAGE_NAME}"
//         //             }

//         //             docker.withRegistry('',DOCKER_PASS) {
//         //                 docker_image.push("${IMAGE_TAG}")
//         //                 docker_image.push('latest')
//         //             }
//         //         }
//         //     }

//         // }
//         // stage('Deploy to k8s'){
//         //     steps{
//         //         script{
//         //             kubernetesDeploy (configs: 'deploymentservice.yaml',kubeconfigId: 'k8sconfigpwd')
//         //         }
//         //     }
//         // }
//         stage('Deploy using docker compose') {
//             steps{
//                 script {
//                     sh "docker-compose -f IdentityProvider.Api/docker-compose.yaml up"
//                 }
//             }
//     }

//     }
// }

pipeline {
    agent any

    environment {
        REPO_URL = 'https://github.com/MohamadAlturky/Membership.git'
        BRANCH = 'dev'
        CREDENTIALS_ID = 'github'
    }

    stages {
        stage('Clone Repository') {
            steps {
                git branch: "${BRANCH}", url: "${REPO_URL}", credentialsId: "${CREDENTIALS_ID}"
            }
        }

        stage('Run Docker Compose') {
            steps {
                script {
                    sh 'docker --version'
                    sh 'ls'
                    sh 'docker stop aspnet_app'
                    sh 'docker rm aspnet_app'
                    sh 'docker rmi identityproviderapi-webapp'
                    sh "docker-compose -f IdentityProvider.Api/docker-compose.yaml up -d"
                }
            }
        }
    }
}