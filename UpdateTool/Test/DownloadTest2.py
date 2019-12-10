from urllib.request import urlretrieve
 
#这是在百度图片里找到一张图片的地址
url = 'https://github.com/Jason-Ma-233/Sakura_DesktopMascot/releases/download/1.4/Sakura_DesktopMascot_1.4a.zip'
 
#下面的go是自定义的回调函数，每次调用它都会打印出当前下载进度
def go(blocknum,blocksize,totalsize):
    percent=blocknum*blocksize/totalsize
    #blocknum是数据块的数量，我只下载一张图片，所以它等于1；blocksize是已经下载的文件大小，totalsize是图片总大小。
    if percent>1:
        percent=1
    #这里用了格式化字符串，输出的格式是小数点后保留两位的百分数 
    print('\r已下载{:.2%}'.format(percent),end='')
    
urlretrieve(url,'GJL.zip',go)
#这个函数用于下载数据。第一个参数是图片的地址；第二个参数是filename，我写的是相对路径，所以会把下载好的图片保存在工作目录里面；第三个参数是reporthook回调函数
