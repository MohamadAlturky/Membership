

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
                    sh "docker-compose -f IdentityProvider.Api/docker-compose.yaml up --build -d"
                }
            }
        }
    }
}