FROM dessix/mono:alpha
MAINTAINER Dessix <Dessix@Dessix.net>

RUN mkdir -p /usr/src/appbuild && mkdir -p /usr/src/app && mkdir -p /usr/src/app/files
COPY ./ /usr/src/appbuild
WORKDIR /usr/src/appbuild
RUN nuget restore DSTHashRepository.sln
RUN xbuild /p:Configuration=Release /tv:12.0 DSTHashRepository.sln

RUN cp -r ./DSTHashRepository/bin/Release/. /usr/src/app/

WORKDIR /usr/src/app

EXPOSE 3000
ENTRYPOINT ["mono", "./DSTHashRepository.exe"]
