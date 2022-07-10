# test-aws-ecs-fargate

## Intro
This project was meant to test ECS services running on Fargate and it's auto-scalability. Basically, I created two dotnet projects, the worker and the sender.

The worker's project is a hosted service that keeps consulting an SQS queue and logging the time it has read the message.

The sender's project is a console has 30 nouns and 30 adjectives pre-configured. It sends 20 batches of 30 messages with nouns and random adjectives.

## AWS Setup
- A SQS Queue.
- A CloudWatch alarm to alert when the queue has more than 20 pending messages.
  - This alert will be used in the service's auto-scaling rule.
- An ECS networking only Cluster.
- A Fargate tesk definition with the worker's docker image.
- An Fargate ECS service with the created task definition.
  - The auto-scaling policy is configured here.
  
## Building Worker's docker image
To build it, just run `docker build -t {your-tag} ./Worker`.
You can publish it to Docker Hub or any container image registry.

## Running
After setting up the ECS service. Just run the Sender project to send 600 messages to the SQS queue.
After that, the amount of unread messages will trigger the CloudWatch's alert and, therefore, will start running another Worker's task.
