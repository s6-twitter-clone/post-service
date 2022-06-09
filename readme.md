# Post service
The post service is in charge of posting, deleting and retrieving posts.
The post service is in charge of the post data and is the only service allowed to alter it.
When a post is created, deleted or otherwise updated the post service is in charge of communicating this change by creating an event on the event bus.

## build instructions
There are two main ways to build and deploy the application.
For local development docker compose is used to run the containers for the service and the database.
For production kubernetes is used. Below there are instructions for both environments.

### Local development
To build and run the project locally you can run `docker compose up`.
This will build the API and run all the services necessary for it to function properly.

### Production
To get the project running in kubernetes there are a couple of steps:
1. build the image for the backend by running 

```bash
$ docker build -f post-service/Dockerfile -t <HOST>/post-service:<TAG>
```

2. Push the image to the docker registry by running 

```bash
$ docker push <HOST>/post-service:<TAG>
```

3. add the service and the database to kubernetes by running 
```bash
$ kubectl apply -f api.yaml -f db.yaml
```

#### deploy.sh and delete.sh
Alternatively you can execute these steps by running the `deploy.sh` script. You can set the host and tag by passing the "h" and "t" flags. For example running
```bash
$ ./deploy.sh -t latest -h localhost:5001
```
will use the `latest` tag and push the image to `localhost:5001`.

If you just want to update the kubernetes configuration you can run 
```bash
$ ./deploy.sh -s
``` 
The `-s` option skips the docker build steps.

To remove a deployment from kubernetes you can run
```bash
$ ./delete.sh
```
The delete script will remove all objects from the kubernetes cluster.